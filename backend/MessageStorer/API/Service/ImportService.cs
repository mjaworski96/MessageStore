using API.Dto;
using API.Persistance.Repository;
using Common.Service;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Service
{
    public interface IImportService
    {
        Task<ImportDtoList> GetImports();
    }
    public class ImportService: IImportService
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
                importsDto.Add(new ImportDto
                {
                    Id = item.ImportId,
                    StartDate = (await _messageRepository.GetOldest(item.Id)).Date,
                    EndDate = (await _messageRepository.GetNewest(item.Id)).Date,
                    Application = await _messageRepository.GetApplication(item.Id)
                });
            }

            return new ImportDtoList
            {
                Imports = importsDto,
            };
        }
    }
}
