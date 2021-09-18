import { Injectable } from '@angular/core';
import {Router} from '@angular/router';
import {HttpErrorResponse} from '@angular/common/http';
import {ToastrService} from 'ngx-toastr';
import {SessionStorageService} from './session-storage.service';

@Injectable({
  providedIn: 'root'
})
export class ErrorHandlingService {

  constructor(private router: Router,
              private toastr: ToastrService,
              private sessionStorageService: SessionStorageService) { }

  handle(error: HttpErrorResponse): void {
    if (error.error) {
      const duplicate = this.toastr.findDuplicate(error.error.message, false, false);
      if (duplicate != null) {
        this.toastr.remove(duplicate.toastId);
      }
      if (error.error.message) {
        this.toastr.error(error.error.message, '', {
          timeOut: 5000,
          closeButton: true
        });
      }
    }
    if (error.status === 404 ||  error.status === 504) {
      this.handle404and504();
    } else if (error.status === 401) {
      this.handle401();
    }
  }
  handle401(): void {
    this.sessionStorageService.logout();
    this.router.navigate(['login']);
  }
  handle404and504(): void {
    this.router.navigate(['not-found']);
  }
}
