import {Injectable} from '@angular/core';
import {HttpEvent, HttpHandler, HttpInterceptor, HttpRequest, HttpResponse} from '@angular/common/http';
import {SessionStorageService} from './services/session-storage.service';
import {Observable, of} from 'rxjs';
import {catchError, finalize, map, tap} from 'rxjs/operators';
import {ErrorHandlingService} from './services/error-handling.service';
import {NgxSpinnerService} from 'ngx-spinner';

@Injectable({
  providedIn: 'root'
})
export class Interceptor  implements HttpInterceptor {

  constructor(private sessionStorageService: SessionStorageService,
              private errorHandlingService: ErrorHandlingService,
              private spinner: NgxSpinnerService) { }

  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    if (this.sessionStorageService.isUserLoggedIn()) {
      request = request.clone({
        setHeaders: {
          Authorization: this.sessionStorageService.getToken()
        }
      });
    }
    const ignoreError = request.url.includes('/api/attachments');
    const ignoreSpinner = request.url.includes('/api/messengerImports/') && request.url.endsWith('/file');
    
    if (!ignoreSpinner) {
      this.spinner.show();
    }

    return next.handle(request).pipe(
      finalize(() => {
        if (!ignoreSpinner) {
          this.spinner.hide();
        }
      }),
      tap(
        result => this.handleValidResponse(result),
        error => this.handleErrorResponse(error, ignoreError))
    );
  }
  handleValidResponse(result: HttpEvent<any>): void {
    if (result instanceof HttpResponse) {
      const response = result as HttpResponse<any>;
      if (response.headers.get('Authorization') !== null && response.headers.get('X-User') !== null) {
        const userJson = response.headers.get('X-User');
        const user = JSON.parse(userJson);
        this.sessionStorageService.storeSession(user, response.headers.get('Authorization'));
      }
    }
  }
  handleErrorResponse(error: any, ignoreErrors: boolean = false): Observable<any> {
    if (!ignoreErrors) {
      this.errorHandlingService.handle(error);
    }
    return of(error);
  }
}
