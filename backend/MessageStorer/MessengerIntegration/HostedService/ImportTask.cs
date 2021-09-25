using MessengerIntegration.Config;
using MessengerIntegration.HostedService.Model;
using MessengerIntegration.Infrastructure;
using MessengerIntegration.Infrastructure.Http;
using MessengerIntegration.Infrastructure.Http.Model;
using MessengerIntegration.Persistance.Entity;
using MessengerIntegration.Persistance.Repository;
using MessengerIntegration.Service;
using Microsoft.Extensions.DependencyInjection;
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
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private IImportRepository _importRepository;
        private IImportService _importService;
        private readonly IFileUtils _fileUtils;
        private readonly IZipFile _zipFile;
        private readonly IAttachmentResolve _attachmentResolve;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IApiConfig _apiConfig;
        private readonly ILogger<ImportTask> _logger;

        private Imports _import;
        private IServiceScope _serviceScope;

        public ImportTask(object syncObject, IImportConfig config, IServiceScopeFactory serviceScopeFactory, IFileUtils fileUtils, IZipFile zipFile, IAttachmentResolve attachmentResolve, IHttpClientFactory httpClientFactory, IApiConfig apiConfig, ILogger<ImportTask> logger)
        {
            Completed = true;
            _syncObject = syncObject;
            _config = config;
            _serviceScopeFactory = serviceScopeFactory;
            _fileUtils = fileUtils;
            _zipFile = zipFile;
            _attachmentResolve = attachmentResolve;
            _httpClientFactory = httpClientFactory;
            _apiConfig = apiConfig;
            _logger = logger;

        }

        public async Task StartImport(string importId)
        {
            Completed = false;
            _cancellationTokenSource = new CancellationTokenSource();
            var token = _cancellationTokenSource.Token;
            PrepareScope();
            _import = await _importRepository.Get(importId, true);
#pragma warning disable CS4014
            Task.Run(async () => await ImportFile(token), token);
#pragma warning restore CS4014

        }
        public async Task Cancel()
        {
            _cancellationTokenSource?.Cancel();
            await SetStatus(Statuses.Queued);
            _serviceScope?.Dispose();
        }
        private async Task ImportFile(CancellationToken cancellationToken)
        {
            var token = new SharedToken();
            var httpClient = _httpClientFactory.CreateClient("apiClient");
            var importApiClient = new ImportApiClient(token, httpClient);
            try
            {
                _logger.LogInformation($"Start import {_import.Id}");
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
                await _importRepository.Save();
                await importApiClient.Finish(_import.Id);
                _serviceScope?.Dispose();
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
        private void PrepareScope()
        {
            _serviceScope = _serviceScopeFactory.CreateScope();
            _importRepository = _serviceScope.ServiceProvider.GetService<IImportRepository>();
            _importService = _serviceScope.ServiceProvider.GetService<IImportService>();
        }
        private async Task ImportConversation(string name,
            List<ZipArchiveEntry> conversationData,
            ContactApiClient contactApiClient,
            MessageApiClient messageApiClient,
            CancellationToken cancellationToken)
        {
            var streamCollection = _zipFile.GetConversationStream(conversationData);
            foreach (var conversationStream in streamCollection)
            {
                using (conversationStream)
                {
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
            }
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
                    ? null : others.Select(x => new ContactMember()
                    { Name = FixEncoding(x.Name), InternalId = FixEncoding(x.Name) }).ToList(),
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
                    ContactMemberId = contact.Members.Count == 1 ? null : contact.Members.FirstOrDefault(x => x.Name == FixEncoding(rawMessage.SenderName))?.Id,
                    ImportId = _import.Id,
                    HasError = rawMessage.IsUnsent ?? false
                };
                await messageApiClient.CreateMessage(messageToSend);
                UpdateImportDates(messageDate);
            }
        }

        private void UpdateImportDates(DateTime messageDate)
        {
            if (!_import.StartDate.HasValue || _import.StartDate.Value > messageDate)
            {
                _import.StartDate = messageDate;
            }
            if (!_import.EndDate.HasValue || _import.EndDate.Value < messageDate)
            {
                _import.EndDate = messageDate;
            }
        }

        private async Task<List<Attachment>> GetAttachments(RawMessage rawMessage, List<ZipArchiveEntry> conversation)
        {
            var result = new List<Attachment>();

            result.AddRange(await GetAttachments(conversation, rawMessage.Photos));
            result.AddRange(await GetAttachments(conversation, rawMessage.Videos));
            result.AddRange(await GetAttachments(conversation, rawMessage.Gifs));
            result.AddRange(await GetAttachments(conversation, rawMessage.Audio));
            result.AddRange(await GetAttachments(conversation, rawMessage.Files));

            return result;
        }
        private async Task<List<Attachment>> GetAttachments(List<ZipArchiveEntry> conversation,
            List<RawAttachment> rawAttachments)
        {
            var result = new List<Attachment>();
            if (rawAttachments == null)
            {
                return result;
            }

            foreach (var attachment in rawAttachments)
            {
                var file = _attachmentResolve.Resolve(conversation, attachment.Uri);
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
                        SaveAsFilename = file.Name
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

            var unescapeText = src.Replace("\\u00", "");
            return Encoding.UTF8.GetString(encoding.GetBytes(unescapeText));
        }
    }
}
