using System.Collections.Generic;

namespace API.Dto
{
    public class SearchQueryDto
    {
        public List<int> AliasesIds { get; set; }
        public string Query { get; set; }
        public bool IgnoreLetterSize { get; set; } = true;
    }
    public class SearchResultDto
    {
        public int MessageId { get; set; }
        public long MessageIndexOf { get; set; }
        public string Content { get; set; }
        public string WriterType { get; set; }
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
