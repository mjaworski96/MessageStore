import { Injectable } from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {MessagesImportList} from '../model/message-import';

@Injectable({
  providedIn: 'root'
})
export class MessagesImportService {
  url = '/api/messagesImports';

  constructor(private http: HttpClient) { }

  async getAll(): Promise<MessagesImportList> {
    return await this.http.get<MessagesImportList>(this.url).toPromise();
  }
  async delete(importId: string): Promise<any> {
    return await this.http.delete(`${this.url}/${importId}/messages`).toPromise();
  }
}
