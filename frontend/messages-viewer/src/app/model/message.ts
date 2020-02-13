import {AttachmentWithID} from './attachment';

export interface MessageWithId {
  id: number;
  content: string;
  attachments: AttachmentWithID[];
  date: string;
  writerType: string;
  contactId: number;
}
export interface MessageWithIdList {
  messages: MessageWithId[];
}
