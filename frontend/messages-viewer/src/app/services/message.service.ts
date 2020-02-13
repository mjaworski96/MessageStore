import { Injectable } from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {Observable} from 'rxjs';
import {MessageWithIdList} from '../model/message';

@Injectable({
  providedIn: 'root'
})
export class MessageService {

  url = '/api/messages';

  constructor(private http: HttpClient) { }

  get(aliasId: number, pageNumber: number, pageSize: number): Observable<MessageWithIdList> {
    return this.http.get<MessageWithIdList>(`${this.url}?aliasId=${aliasId}&pageNumber=${pageNumber}&pageSize=${pageSize}`);
  }
}
