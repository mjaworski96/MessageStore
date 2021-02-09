import { Injectable } from '@angular/core';
import {ActivatedRouteSnapshot, Resolve, RouterStateSnapshot} from '@angular/router';
import {LoggedUser} from '../../../model/user';
import {Observable} from 'rxjs';
import {UserAccountService} from './user-account.service';

@Injectable({
  providedIn: 'root'
})
export class UserAccountResolveService implements Resolve<LoggedUser> {

  constructor(private userService: UserAccountService) { }

  resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot):
    Observable<LoggedUser> | Promise<LoggedUser> | LoggedUser {
    return this.userService.getUser()
      .toPromise()
      .then(result => {
        return result;
      });
  }
}
