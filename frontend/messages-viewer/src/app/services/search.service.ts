import { Injectable } from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {Query, SearchResultDtoList} from '../model/search';
import {Observable} from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class SearchService {
  url = '/api/messages/search';
  constructor(private http: HttpClient) { }

  search(query: Query): Observable<SearchResultDtoList> {
    return this.http.post<SearchResultDtoList>(this.url, query);
  }
}
