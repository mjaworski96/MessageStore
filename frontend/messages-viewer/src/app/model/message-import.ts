export class MessagesImport {
  id: string;
  application: string;
  isBeingDeleted: boolean;
  createdAt: string;
  startDate: string;
  endDate: string;
}
export class MessagesImportList {
  imports: MessagesImport[];
}
