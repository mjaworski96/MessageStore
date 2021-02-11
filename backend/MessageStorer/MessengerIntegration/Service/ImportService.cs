using Common.Exceptions;
using Common.Service;
using MessengerIntegration.Config;
using MessengerIntegration.Dto;
using MessengerIntegration.Infrastructure;
using MessengerIntegration.Persistance.Entity;
using MessengerIntegration.Persistance.Repository;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MessengerIntegration.Service
{
    public interface IImportService
    {
        Task<ImportDtoWithId> StartProcess(ImportDto importDto);
        Task UploadFile(string importId, FileDto fileDto);
        Task FinishUpload(string importId);
        Task<ImportDtoWithIdList> GetAllForUser();
    }
    public class ImportService: IImportService
    {
        private readonly IImportRepository _importRepository;
        private readonly IStatusRepository _statusRepository;
        private readonly IHttpMetadataService _httpMetadataService;
        private readonly IFileUpload _fileUpload;

        public ImportService(IImportRepository importRepository, 
            IStatusRepository statusRepository,
            IHttpMetadataService httpMetadataService,
            IFileUpload fileUpload)
        {
            _importRepository = importRepository;
            _statusRepository = statusRepository;
            _httpMetadataService = httpMetadataService;
            _fileUpload = fileUpload;
        }

        public async Task<ImportDtoWithId> StartProcess(ImportDto importDto)
        {
            if (string.IsNullOrEmpty(importDto.FbUsername))
            {
                throw new BadRequestException("Facebook usernname must be provided");
            }
            if (importDto.FbUsername.Length > 256)
            {
                throw new BadRequestException("Facebook usernname length must be less than 256");
            }
            var importEntity = new Imports
            {
                Id = Guid.NewGuid().ToString(),
                Status = await _statusRepository.GetStatusByName(Statuses.Created),
                UserId = _httpMetadataService.UserId,
                FbUsername = importDto.FbUsername
            };
            await _importRepository.Add(importEntity);
            await _importRepository.Save();
            return CreateImportDtoWithId(importEntity);
        }

        public async Task UploadFile(string importId, FileDto fileDto)
        {
            var import = await _importRepository.Get(importId);
            CheckImport(import);
            CheckStatus(import, Statuses.Created);
            try
            {
                await _fileUpload.Upload(import.Id, fileDto.Content);
            } 
            catch(Exception e)
            {
                await SetStatus(import, Statuses.ErrorUnknownError);
                throw e;
            }
        }
        public async Task FinishUpload(string importId)
        {
            var import = await _importRepository.Get(importId);
            CheckImport(import);
            CheckStatus(import, Statuses.Created);
            try
            {
                await SetStatus(import, Statuses.Queued);
            }
            catch (Exception e)
            {
                await SetStatus(import, Statuses.ErrorUnknownError);
                throw e;
            }
        }
        private async Task SetStatus(Imports import, string statusName)
        {
            import.Status = await _statusRepository.GetStatusByName(statusName);
            await _importRepository.Save();
        }

        private void CheckStatus(Imports import, string validStatus)
        {
            if (import.Status.Name != validStatus)
            {
                throw new BadRequestException($"Import has invalid status. " +
                    $"Should be {validStatus} but is {import.Status.Name}");
            }
        }
        private void CheckImport(Imports import)
        {
            if (import.UserId != _httpMetadataService.UserId)
            {
                throw new ForbiddenException($"You can't acces this import");
            }
        }
        private ImportDtoWithId CreateImportDtoWithId(Imports importEntity)
        {
            return new ImportDtoWithId
            {
                Id = importEntity.Id,
                FbUsername = importEntity.FbUsername,
                StartDate = importEntity.StartDate,
                EndDate = importEntity.EndDate,
                Status = importEntity.Status.Name
            };
        }

        public async Task<ImportDtoWithIdList> GetAllForUser()
        {
            var entities = await _importRepository.GetAll(_httpMetadataService.UserId);
            return new ImportDtoWithIdList
            {
                Imports = entities.Select(x => CreateImportDtoWithId(x)).ToList()
            };
        }
    }
}
