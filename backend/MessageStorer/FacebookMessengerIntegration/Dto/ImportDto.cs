using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FacebookMessengerIntegration.Dto
{
    public class ImportDto
    {
        public string FbUsername { get; set; }
    }
    public class ImportDtoWithId
    {
        public string Id { get; set; }
        public string FbUsername { get; set; }
        public string Status { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
    public class ImportListDto
    {
        public List<ImportDto> Imports { get; set; }
    }
}
