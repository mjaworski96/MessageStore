export class StartImport {
  facebookName: string;
}
export class Import {
  id: string;
  facebookName: string;
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
