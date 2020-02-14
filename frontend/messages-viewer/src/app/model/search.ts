export interface Query {
  query: string;
  ignoreLetterSize: boolean;
  aliasesIds: number[];
}
export interface SearchAlias {
  id: number;
  name: string;
  messageIndexOf: number;
}
export interface SearchResultDto {
  messageId: number;
  messageIndexOf: number;
  content: string;
  writerType: string;
  contactName: string;
  aliasId: number;
  application: string;
  allAliases: SearchAlias[];
}
export interface SearchResultDtoList {
  results: SearchResultDto[];
}
