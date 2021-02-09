import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import {FileUpload, Import, StartImport} from '../model/import';
import {Observable} from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ImportService {
  url = '/api/import';

  constructor(private httpClient: HttpClient) { }

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
