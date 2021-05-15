using API.Config;
using API.Dto;
using API.Persistance.Entity;
using API.Persistance.Repository;
using HeyRed.Mime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace API.Service
{
    public interface IAttachmentService
    {
        Task<List<Attachments>> CreateAttachments(List<AttachmentDto> attachments);
        Task<AttachmentContentDto> Get(int id);
        Task<AttachmentMetadataDto> GetMetadata(int id);
        AttachmentDtoWithId CreateAttachemtDtoWithId(Attachments attachment);
        void Remove(string filename);
    }
    public class AttachmentService : IAttachmentService
    {
        private readonly IAttachmentConfig _attachmentsConfig;
        private readonly IAttachmentRepository _attachmentRepository;
        private readonly ISecurityService _securityService;

        public AttachmentService(IAttachmentConfig attachmentsConfig, IAttachmentRepository attachmentRepository, ISecurityService securityService)
        {
            _attachmentsConfig = attachmentsConfig;
            _attachmentRepository = attachmentRepository;
            _securityService = securityService;
        }

        public async Task<List<Attachments>> CreateAttachments(List<AttachmentDto> attachments)
        {
            var result = new List<Attachments>();

            foreach (var item in attachments ?? Enumerable.Empty<AttachmentDto>())
            {
                result.Add(await CreateAttachment(item));
            }

            return result;
        }

        private async Task<Attachments> CreateAttachment(AttachmentDto attachmentDto)
        {
            var filename = Guid.NewGuid().ToString();
            await File.WriteAllBytesAsync(Path.Combine(_attachmentsConfig.Directory, filename), attachmentDto.Content);
            return new Attachments
            {
                ContentType = attachmentDto.ContentType,
                Filename = filename,
                SaveAsFilename = attachmentDto.SaveAsFilename ?? $"{filename}.{MimeTypesMap.GetExtension(attachmentDto.ContentType)}"
            };
        }

        public AttachmentDtoWithId CreateAttachemtDtoWithId(Attachments attachment)
        {
            return new AttachmentDtoWithId
            {
                Id = attachment.Id,
                ContentType = attachment.ContentType,
                SaveAsFilename = attachment.SaveAsFilename
            };
        }
        public async Task<AttachmentContentDto> Get(int id)
        {
            var entity = await _attachmentRepository.Get(id);
            _securityService.CheckIfUserIsOwnerOfAttachment(entity);
            var content = new byte[0];

            if (!string.IsNullOrEmpty(entity.Filename))
            {
                content = await File.ReadAllBytesAsync(Path.Combine(_attachmentsConfig.Directory, entity.Filename));
            }

            return new AttachmentContentDto
            {
                Content = content,
                ContentType = entity.ContentType,
                SaveAsFilename = entity.SaveAsFilename
            };
        }

        public async Task<AttachmentMetadataDto> GetMetadata(int id)
        {
            var entity = await _attachmentRepository.Get(id);
            _securityService.CheckIfUserIsOwnerOfAttachment(entity);
            var relativePath = Path.Combine(_attachmentsConfig.Directory, entity.Filename);
            return new AttachmentMetadataDto
            {
                Filename = Path.GetFullPath(relativePath),
                ContentType = entity.ContentType,
                SaveAsFilename = entity.SaveAsFilename
            };
        }

        public void Remove(string filename)
        {
            File.Delete(Path.Combine(_attachmentsConfig.Directory, filename));
        }
    }
}
