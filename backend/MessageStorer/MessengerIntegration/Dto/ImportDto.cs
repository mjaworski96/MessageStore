using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MessengerIntegration.Dto
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
    public class ImportDtoWithIdList
    {
        public List<ImportDtoWithId> Imports { get; set; }
    }
}
