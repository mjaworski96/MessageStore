export class StartImport {
  fbUsername: string;
}
export class Import {
  id: string;
  fbUsername: string;
  status: string;
  startDate: string;
  endDate: string;
}
export class ImportsList {
  imports: Import[];
}
export class FileUpload {
  content: string;
}
