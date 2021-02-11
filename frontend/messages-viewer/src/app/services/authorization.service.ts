import { Injectable } from '@angular/core';
import {HttpClient, HttpResponse} from '@angular/common/http';
import {SessionStorageService} from './session-storage.service';
import {ErrorHandlingService} from './error-handling.service';
import {Router} from '@angular/router';
import {LoggedUser, LoginDetails, RegisterUserDetails} from '../model/user';

@Injectable({
  providedIn: 'root'
})
export class AuthorizationService {
  url = '/api/AppUsers';

  constructor(private httpClient: HttpClient,
              private sessionStorage: SessionStorageService,
              private errorHandlingService: ErrorHandlingService,
              private router: Router) { }
  handleValidUser(response: HttpResponse <LoggedUser>): void {
    this.sessionStorage.storeSession(response.body,
      response.headers.get('Authorization'));
    this.router.navigate(['/', 'aliases']);
  }
  login(loginDetails: LoginDetails): void {
    this.httpClient.post(`${this.url}/login`, loginDetails, {observe: 'response'})
      .toPromise().then( (response: HttpResponse <LoggedUser>) => {
      this.handleValidUser(response);
    });
  }
  register(registerDetails: RegisterUserDetails): void {
    this.httpClient.post(this.url, registerDetails, {observe: 'response'})
      .toPromise().then( (response: HttpResponse <LoggedUser>) => {
      this.handleValidUser(response);
    });
  }
}
