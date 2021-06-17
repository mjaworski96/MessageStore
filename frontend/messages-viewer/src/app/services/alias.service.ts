import { Injectable } from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {Observable} from 'rxjs';
import {AliasWithId, AliasWithIdList, EditAlias, EditAliasName} from '../model/alias';

@Injectable({
  providedIn: 'root'
})
export class AliasService {
  url = '/api/aliases';

  constructor(private http: HttpClient) { }

  getAll(internalOnly: boolean = false): Observable<AliasWithIdList> {
    return this.http.get<AliasWithIdList>(`${this.url}?internalOnly=${internalOnly}`);
  }
  create(alias: EditAlias): Observable<AliasWithId> {
    return this.http.post<AliasWithId>(this.url, alias);
  }
  get(id: number): Observable<AliasWithId> {
    return this.http.get<AliasWithId>(`${this.url}/${id}`);
  }
  edit(id: number, alias: EditAlias): Observable<AliasWithId> {
    return this.http.put<AliasWithId>(`${this.url}/${id}`, alias);
  }
  editName(id: number, aliasName: EditAliasName): Observable<AliasWithId> {
    return this.http.patch<AliasWithId>(`${this.url}/${id}/name`, aliasName);
  }
  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.url}/${id}`);
  }
}
