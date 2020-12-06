using API.Dto;
using API.Exceptions;
using API.Persistance.Entity;
using API.Persistance.Repository;
using System.Linq;
using System.Threading.Tasks;

namespace API.Service
{
    public interface IAliasService
    {
        Task<AliasDtoWithIdList> GetAll(string app, bool internalOnly);
        Task<AliasDtoWithId> Create(CreateAliasDto createAlias);
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
                .Select(x => CreateAliasDtoWithId(x)).ToList();
            return new AliasDtoWithIdList
            {
                Aliases = list
            };
        }
        
        public async Task<AliasDtoWithId> Create(CreateAliasDto createAlias)
        {
            var contacts = await _aliasRepository.GetAll(createAlias.Members.Select(x => x.Id));
            if (contacts.Count != createAlias.Members.Count)
            {
                throw new NotFoundException("Contact not found");
            }
            if (contacts.Any(x => !x.Internal))
            {
                throw new BadRequestException("Aliases can be created only from contacts (not other aliases)");
            }
            var alias = new Aliases
            {
                Name = createAlias.Name,
                Internal = false,
            };
            alias.AliasesMembers = contacts.Select(
                x => new AliasesMembers
                {
                    Alias = alias,
                    Contact = x.AliasesMembers.First().Contact
                }).ToList();
            await _aliasRepository.Add(alias);
            return CreateAliasDtoWithId(alias);
        }


        private static AliasDtoWithId CreateAliasDtoWithId(Aliases alias)
        {
            return new AliasDtoWithId
            {
                Id = alias.Id,
                Name = alias.Name,
                Internal = alias.Internal,
                Application = alias.Internal ? alias.AliasesMembers.First().Contact.Application.Name : "",
                InApplicationId = alias.Internal ? alias.AliasesMembers.First().Contact.InApplicationId : "",
                Members = alias.AliasesMembers.Select(y => new AliasMemberDtoWithId
                {
                    Id = y.Contact.Id,
                    Name = y.Contact.Name,
                    Application = y.Contact.Application.Name,
                    InApplicationId = y.Contact.InApplicationId
                }).ToList()
            };
        }

    }
}
