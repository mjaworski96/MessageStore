using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MessengerIntegration.Dto
{
    public class ImportDto
    {
        public string FacebookName { get; set; }
    }
    public class ImportDtoWithId
    {
        public string Id { get; set; }
        public string FacebookName { get; set; }
        public string Status { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
    public class ImportDtoWithIdList
    {
        public List<ImportDtoWithId> Imports { get; set; }
    }
}
