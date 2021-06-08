import {AttachmentWithID} from './attachment';

export interface Query {
  query: string;
  from: string;
  to: string;
  hasAttachments: boolean;
  ignoreLetterSize: boolean;
  aliasesIds: number[];
}
export interface SearchAlias {
  id: number;
  name: string;
}
export interface SearchResultDto {
  messageId: number;
  content: string;
  attachments: AttachmentWithID[];
  date: string;
  writerType: string;
  hasError: boolean;
  contactName: string;
  aliasId: number;
  application: string;
  allAliases: SearchAlias[];
}
export interface SearchResultDtoList {
  results: SearchResultDto[];
}
