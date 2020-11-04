import { Injectable } from '@angular/core';
import {Router} from '@angular/router';
import {HttpErrorResponse} from '@angular/common/http';
import {ToastrService} from 'ngx-toastr';

@Injectable({
  providedIn: 'root'
})
export class ErrorHandlingService {

  constructor(private router: Router,
              private toastr: ToastrService) { }

  handle(error: HttpErrorResponse): void {
    console.log(error);
    if (error.error !== undefined && error.error !== null) {
      const duplicate = this.toastr.findDuplicate(error.error.message, false, false);
      if (duplicate != null) {
        this.toastr.remove(duplicate.toastId);
      }
      this.toastr.error(error.error.message, error.error.code, {
        timeOut: 5000,
        closeButton: true
      });
    }
    if (error.error.code === 404 ||  error.error.code === 504) {
      this.handle404and504();
    } else if (error.error.code === 401) {
      this.handle401();
    }
  }
  handle401(): void {
    // TODO: authorization
  }
  handle404and504(): void {
    this.router.navigate(['not-found']);
  }
}
