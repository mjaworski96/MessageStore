using System;
using System.Collections.Generic;

namespace API.Dto
{
    public class SearchQueryDto
    {
        public List<int> AliasesIds { get; set; }
        public string Query { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public bool IgnoreLetterSize { get; set; } = true;
        public bool HasAttachments { get; set; } = false;
    }
    public class SearchResultDto
    {
        public int MessageId { get; set; }
        public long MessageIndexOf { get; set; }
        public string Content { get; set; }
        public List<AttachmentDtoWithId> Attachments { get; set; }
        public DateTime Date { get; set; }
        public string WriterType { get; set; }
        public bool HasError { get; set; }
        public string ContactName { get; set; }
        public int AliasId { get; set; }
        public string Application { get; set; }
        public List<SearchAlias> AllAliases { get; set; }
    }
    public class SearchAlias
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public long MessageIndexOf { get; set; }
    }
    public class SearchResultDtoList
    {
        public List<SearchResultDto> Results { get; set; }
    }
}
