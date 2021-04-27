import { Injectable } from '@angular/core';
import {HttpClient} from '@angular/common/http';
import { saveAs } from 'file-saver';

@Injectable({
  providedIn: 'root'
})
export class AttachmentService {

  constructor(private http: HttpClient) { }

  downloadAttachment(id: number, filename: string) {
    this.http.get(`/api/attachments/${id}/stream`, {responseType: 'blob'})
      .toPromise()
      .then(result => {
        saveAs(result, filename);
      });
  }
}
