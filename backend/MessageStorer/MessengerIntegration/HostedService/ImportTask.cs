using MessengerIntegration.Config;
using MessengerIntegration.HostedService.Model;
using MessengerIntegration.Infrastructure;
using MessengerIntegration.Infrastructure.Http;
using MessengerIntegration.Infrastructure.Http.Model;
using MessengerIntegration.Persistance.Entity;
using MessengerIntegration.Persistance.Repository;
using MessengerIntegration.Service;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Threading;
using System.Threading.Tasks;

namespace MessengerIntegration.HostedService
{
    public class ImportTask
    {
        public bool Completed { get; set; }
        private CancellationTokenSource _cancellationTokenSource;
        private readonly object _syncObject;
        private readonly IImportConfig _config;
        private readonly IImportRepository _importRepository;
        private readonly IImportService _importService;
        private readonly IFileUtils _fileUtils;
        private readonly IZipFile _zipFile;
        private readonly IAttachmentResolve _attachmentResolve;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IApiConfig _apiConfig;
        private readonly ILogger<ImportTask> _logger;

        private Imports _import;

        public ImportTask(object syncObject, IImportConfig config, IImportRepository importRepository, IImportService importService, IFileUtils fileUtils, IZipFile zipFile, IAttachmentResolve attachmentResolve, IHttpClientFactory httpClientFactory, IApiConfig apiConfig, ILogger<ImportTask> logger)
        {
            Completed = true;
            _syncObject = syncObject;
            _config = config;
            _importRepository = importRepository;
            _importService = importService;
            _fileUtils = fileUtils;
            _zipFile = zipFile;
            _attachmentResolve = attachmentResolve;
            _httpClientFactory = httpClientFactory;
            _apiConfig = apiConfig;
            _logger = logger;

        }

        public void StartImport(Imports import)
        {
            Completed = false;
            _import = import;
            _cancellationTokenSource = new CancellationTokenSource();
            var token = _cancellationTokenSource.Token;
            Task.Run(async () => await ImportFile(token), token);

        }
        public async Task Cancel()
        {
            _cancellationTokenSource?.Cancel();
            await SetStatus(Statuses.Queued);
        }
        private async Task ImportFile(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Start import {_import.Id}");
            try
            {
                await SetStatus(Statuses.Processing);
                cancellationToken.ThrowIfCancellationRequested();

                using var stream = _zipFile.Open(_import);
                using var zip = _zipFile.Open(stream);
                var messages = _zipFile.GetMessages(zip);
                cancellationToken.ThrowIfCancellationRequested();

                if (!messages.Any())
                {
                    _logger.LogError($"No messages for import: {_import.Id}");
                    await SetStatus(Statuses.ErrorNoMessages);
                }

                var token = new SharedToken();
                var httpClient = _httpClientFactory.CreateClient("apiClient");
                var userApiClient = new UserApiClient(token, httpClient, _apiConfig);
                var contactApiClient = new ContactApiClient(token, httpClient);
                var messageApiClient = new MessageApiClient(token, httpClient);

                await userApiClient.Authorize(_import.UserId);
                foreach (var conversation in messages)
                {
                    await ImportConversation(conversation.Key, conversation.Value, contactApiClient, messageApiClient, cancellationToken);
                }


                await SetStatus(Statuses.Completed);
                _logger.LogInformation($"Finished import {_import.Id}");
            }
            catch (InvalidDataException e)
            {
                _logger.LogError($"Corrupted file for import: {_import.Id}: {e.Message}\n{e.StackTrace}");
                await SetStatus(Statuses.ErrorInvalidFile);
            }
            catch (Exception e)
            {
                _logger.LogError($"Exception occured while importing file for import: {_import.Id}: {e.Message}\n{e.StackTrace}");
                await SetStatus(Statuses.ErrorUnknownError);
            }
            finally
            {
                if (_config.DeleteFileAfterImport && _import != null)
                {
                    _fileUtils.Delete(_import.Id);
                }
                lock (_syncObject)
                {
                    Completed = true;
                }
            }

        }
        private async Task SetStatus(string statusName)
        {
            if (!Completed && _import != null)
            {
                await _importService.SetStatus(_import, statusName);
            }
        }
        private async Task ImportConversation(string name,
            List<ZipArchiveEntry> conversationData,
            ContactApiClient contactApiClient,
            MessageApiClient messageApiClient,
            CancellationToken cancellationToken)
        {
            using var conversationStream = _zipFile.GetConversationStream(conversationData);

            cancellationToken.ThrowIfCancellationRequested();
            if (conversationStream == null)
            {
                _logger.LogWarning($"Empty converstion {name} in import {_import.Id}");
                return;
            }
            var conversationDocument = await _zipFile.GetConversation(conversationStream);
            cancellationToken.ThrowIfCancellationRequested();

            var contact = await ImportContact(name, conversationDocument, contactApiClient);
            cancellationToken.ThrowIfCancellationRequested();
            await ImportMessages(contact, messageApiClient, conversationDocument, conversationData, cancellationToken);
        }

        private async Task<ContactWithId> ImportContact(string conversationName, JsonDocument document,
            ContactApiClient contactApiClient)
        {
            var participantsRaw = document.RootElement.GetProperty("participants");
            var participants = JsonConvert.DeserializeObject<List<Participant>>(participantsRaw.GetRawText());
            var others = participants.Where(x => FixEncoding(x.Name) != _import.FacebookName).ToList();
            var contact = new Contact()
            {
                InApplicationId = conversationName,
                Name = others.Count == 1
                    ? FixEncoding(others.First().Name) : conversationName,
                Members = others.Count == 1
                    ? null : others.Select(x => new ContactMember() { Name = FixEncoding(x.Name) }).ToList(),
            };
            return await contactApiClient.CreateOrUpdateContact(contact);
        }
        private async Task ImportMessages(ContactWithId contact, MessageApiClient messageApiClient,
            JsonDocument document, List<ZipArchiveEntry> conversationData,
            CancellationToken cancellationToken)
        {
            var lastSyncTime = await messageApiClient.GetSyncTime(contact.Id);
            var messages = document.RootElement.GetProperty("messages");
            foreach (var message in messages.EnumerateArray())
            {
                await ImportMessages(contact, messageApiClient, message, conversationData, lastSyncTime);
                cancellationToken.ThrowIfCancellationRequested();
            }
        }

        private async Task ImportMessages(ContactWithId contact, MessageApiClient messageApiClient, JsonElement message, List<ZipArchiveEntry> conversationData, SyncDateTime lastSyncTime)
        {
            var rawMessage = JsonConvert.DeserializeObject<RawMessage>(message.GetRawText());
            var messageDate = new DateTime(1970, 1, 1).AddMilliseconds(rawMessage.TimestampMs);
            if (messageDate < lastSyncTime.From || messageDate > lastSyncTime.To)
            {
                var messageToSend = new Message
                {
                    ContactId = contact.Id,
                    Content = FixEncoding(rawMessage.Content),
                    Attachments = await GetAttachments(rawMessage, conversationData),
                    Date = messageDate,
                    WriterType = rawMessage.SenderName == _import.FacebookName ? "app_user" : "contact",
                    ContactMemberId = contact.Members.Count == 1 ? null : contact.Members.FirstOrDefault(x => x.Name == FixEncoding(rawMessage.SenderName))?.Id
                };
                await messageApiClient.CreateMessage(messageToSend);
            }
        }
        private async Task<List<Attachment>> GetAttachments(RawMessage rawMessage, List<ZipArchiveEntry> conversation)
        {
            var result = new List<Attachment>();

            result.AddRange(await GetAttachments(rawMessage.Photos, 
                (name) => _attachmentResolve.ResolveForPhoto(conversation, name)));

            result.AddRange(await GetAttachments(rawMessage.Videos,
                (name) => _attachmentResolve.ResolveForVideo(conversation, name)));

            result.AddRange(await GetAttachments(rawMessage.Gifs,
                (name) => _attachmentResolve.ResolveForGif(conversation, name)));

            result.AddRange(await GetAttachments(rawMessage.Audio,
                (name) => _attachmentResolve.ResolveForAudio(conversation, name)));

            return result;
        }
        private async Task<List<Attachment>> GetAttachments(List<RawAttachment> rawAttachments,
            Func<string, ZipArchiveEntry> resolver)
        {
            var result = new List<Attachment>();
            if (rawAttachments == null)
            {
                return result;
            }

            foreach (var attachment in rawAttachments)
            {
                var file = resolver(attachment.Uri);
                if (file != null)
                {
                    using var stream = file.Open();
                    using var buffer = new MemoryStream();
                    await stream.CopyToAsync(buffer);
                    var mime = _attachmentResolve.GetMimeType(file.Name);
                    result.Add(new Attachment
                    {
                        Content = buffer.ToArray(),
                        ContentType = mime,
                    });
                }
            }

            return result;
        }
        private string FixEncoding(string src)
        {
            if (string.IsNullOrEmpty(src))
            {
                return src;
            }
            var encoding = Encoding.GetEncoding(_config.FileEncoding);
            var unescapeText = System.Text.RegularExpressions.Regex.Unescape(src);
            return Encoding.UTF8.GetString(encoding.GetBytes(unescapeText));
        }
    }
}
