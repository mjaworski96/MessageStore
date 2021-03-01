import {AttachmentWithID} from './attachment';

export interface MessageWithId {
  id: number;
  content: string;
  attachments: AttachmentWithID[];
  date: string;
  writerType: string;
  contactName: string;
  application: string;
}
export interface MessageWithIdList {
  messages: MessageWithId[];
}
