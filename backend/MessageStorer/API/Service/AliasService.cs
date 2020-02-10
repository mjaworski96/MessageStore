using API.Dto;
using API.Persistance.Repository;
using System.Linq;
using System.Threading.Tasks;

namespace API.Service
{
    public interface IAliasService
    {
        Task<AliasDtoWithIdList> GetAll(string app, bool internalOnly);
    }
    public class AliasService : IAliasService
    {
        private readonly IAliasRepository _aliasRepository;
        private readonly IHttpMetadataService _httpMetadataService;

        public AliasService(IAliasRepository aliasRepository, IHttpMetadataService httpMetadataService)
        {
            _aliasRepository = aliasRepository;
            _httpMetadataService = httpMetadataService;
        }

        public async Task<AliasDtoWithIdList> GetAll(string app, bool internalOnly)
        {
            var rawEntities = await _aliasRepository.GetAll(
                _httpMetadataService.Username, app, internalOnly);

            var list = rawEntities
                .Select(x => new AliasDtoWithId
                {
                    Id = x.Id,
                    Name = x.Name,
                    Internal = x.Internal,
                    InApplicationId = x.Internal ? x.AliasesMembers.First().Contact.InApplicationId : ""
                });
            return new AliasDtoWithIdList
            {
                Aliases = list.ToList()
            };
        }
    }
}
