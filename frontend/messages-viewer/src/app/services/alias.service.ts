import { Injectable } from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {Observable} from 'rxjs';
import {AliasWithIdList} from '../app/model/alias';

@Injectable({
  providedIn: 'root'
})
export class AliasService {
  url = '/api/aliases';

  constructor(private http: HttpClient) { }

  getAll(): Observable<AliasWithIdList> {
    return this.http.get<AliasWithIdList>(this.url);
  }
}
