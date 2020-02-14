import { Injectable } from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {Query, SearchResultDtoList} from '../model/search';
import {Observable} from 'rxjs';
import {MessageInAliasOrder} from '../model/message-in-alias-order';

@Injectable({
  providedIn: 'root'
})
export class SearchService {
  urlSearch = '/api/messages/search';
  urlOrder = '/api/messages/order';
  constructor(private http: HttpClient) { }

  search(query: Query): Observable<SearchResultDtoList> {
    return this.http.post<SearchResultDtoList>(this.urlSearch, query);
  }
  getOrder(messageId: number, aliasId: number): Observable<MessageInAliasOrder> {
    return this.http.get<MessageInAliasOrder>(`${this.urlOrder}?messageId=${messageId}&aliasId=${aliasId}`);
  }
}
