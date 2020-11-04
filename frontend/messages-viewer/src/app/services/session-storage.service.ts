import { Injectable } from '@angular/core';
import {LoggedUser} from '../model/user';

@Injectable({
  providedIn: 'root'
})
export class SessionStorageService {

  constructor() { }

  storeSession(user: LoggedUser, token: string): void {
    localStorage.setItem('token', token);
    localStorage.setItem('userData', JSON.stringify(user));
  }
  updateSession(token: string): void {
    localStorage.setItem('token', token);
  }
  clearSession(): void {
    localStorage.clear();
  }
  logout(): void {
    localStorage.removeItem('token');
    localStorage.removeItem('userData');
  }
  getUser(): LoggedUser {
    return JSON.parse(localStorage.getItem('userData'));
  }
  getToken(): string {
    return localStorage.getItem('token');
  }
  isUserLoggedIn(): boolean {
    return this.getUser() !== null;
  }
}
