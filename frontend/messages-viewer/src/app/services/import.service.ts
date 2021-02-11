import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import {FileUpload, Import, ImportsList, StartImport} from '../model/import';

@Injectable({
  providedIn: 'root'
})
export class ImportService {
  url = '/api/import';

  constructor(private httpClient: HttpClient) { }

  getAll(): Promise<ImportsList> {
    return this.httpClient.get<ImportsList>(this.url)
      .toPromise();
  }
  start(startImport: StartImport): Promise<Import> {
    return this.httpClient.post<Import>(this.url, startImport)
      .toPromise();
  }
  fileUpload(importId, fileUpload: FileUpload): Promise<Import> {
    return this.httpClient.post<Import>(`${this.url}/${importId}/file`, fileUpload)
      .toPromise();
  }
  finish(importId: string): Promise<Import> {
    return this.httpClient.put<Import>(`${this.url}/${importId}`, null)
      .toPromise();
  }
}
