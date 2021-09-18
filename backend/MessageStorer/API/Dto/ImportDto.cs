using System;
using System.Collections.Generic;

namespace API.Dto
{
    public class ImportDto
    {
        public string Id { get; set; }
        public string Application { get; set; }
        public bool IsBeingDeleted { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
    public class ImportDtoList
    {
        public List<ImportDto> Imports { get; set; }
    }
}
