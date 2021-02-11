import {Injectable} from '@angular/core';
import {CanActivate, Router} from '@angular/router';
import {SessionStorageService} from '../../services/session-storage.service';

@Injectable({
  providedIn: 'root'
})
export class NotLoggedUserGuard implements CanActivate {
  constructor(private sessionStorageService: SessionStorageService,
              private router: Router) {}

  canActivate(): boolean {
    if (!this.sessionStorageService.isUserLoggedIn()) {
      return true;
    } else {
      this.router.navigate(['/messages']);
      return false;
    }
  }
}
