using API.Dto;
using API.Persistance.Repository;
using Common.Exceptions;
using Common.Service;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Service
{
    public interface IImportService
    {
        Task<ImportDtoList> GetImports();
        Task Finish(string importId);
        Task<ImportDtoList> RefreshDates();
    }
    public class ImportService : IImportService
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IImportRepository _importRepository;
        private readonly IHttpMetadataService _httpServiceMetadataService;

        public ImportService(IMessageRepository messageRepository, IImportRepository importRepository, IHttpMetadataService httpServiceMetadataService)
        {
            _messageRepository = messageRepository;
            _importRepository = importRepository;
            _httpServiceMetadataService = httpServiceMetadataService;
        }

        public async Task<ImportDtoList> GetImports()
        {
            var imports = await _importRepository.GetAllForUser(_httpServiceMetadataService.UserId);
            var importsDto = new List<ImportDto>();

            foreach (var item in imports)
            {
                importsDto.Add(CreateImportDto(item));
            }

            return new ImportDtoList
            {
                Imports = importsDto,
            };
        }

        public async Task<ImportDtoList> RefreshDates()
        {
            var imports = await _importRepository.GetAllForUser(_httpServiceMetadataService.UserId);
            var importsDto = new List<ImportDto>();

            foreach (var item in imports)
            {
                item.StartDate = (await _messageRepository.GetOldest(item.Id, false)).Date;
                item.EndDate = (await _messageRepository.GetNewest(item.Id, false)).Date;

                importsDto.Add(CreateImportDto(item));
            }

            await _importRepository.Commit();

            return new ImportDtoList
            {
                Imports = importsDto,
            };
            throw new System.NotImplementedException();
        }

        public async Task Finish(string importId)
        {
            var import = await _importRepository.Get(importId);
            if (import == null)
            {
                throw new ImportNotFoundException(importId);
            }
            import.StartDate = (await _messageRepository.GetOldest(import.Id, false)).Date;
            import.EndDate = (await _messageRepository.GetNewest(import.Id, false)).Date;
            await _importRepository.Commit();
        }

        private ImportDto CreateImportDto(Persistance.Entity.Imports item)
        {
            return new ImportDto
            {
                Id = item.ImportId,
                CreatedAt = item.CreatedAt,
                IsBeingDeleted = item.IsBeingDeleted,
                StartDate = item.StartDate,
                EndDate = item.EndDate,
                Application = item.Application.Name
            };
        }
    }
}
