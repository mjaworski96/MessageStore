import { Injectable } from '@angular/core';
import {Observable} from 'rxjs';
import {HttpClient, HttpResponse} from '@angular/common/http';
import {LoggedUser, PasswordChange, UserUpdateData} from '../model/user';

@Injectable({
  providedIn: 'root'
})
export class UserAccountService {
  url = '/api/AppUsers';

  constructor(private httpClient: HttpClient) { }

  getUser(): Observable<LoggedUser> {
    return this.httpClient.get<LoggedUser>(
      `${this.url}`
    );
  }
  deleteAccount(): Observable<any> {
    return this.httpClient.delete(
      `${this.url}`
    );
  }
  updateAccount(updateData: UserUpdateData): Observable<HttpResponse<LoggedUser>> {
    return this.httpClient.put<LoggedUser>(
      `${this.url}`,
      updateData,
      {observe: 'response'}
    );
  }
  changePassword(passwordChange: PasswordChange): Observable<any> {
    return this.httpClient.post(
      `${this.url}/passwordChange`,
      passwordChange
    );
  }
}
