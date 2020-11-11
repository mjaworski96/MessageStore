import {Injectable} from '@angular/core';
import {HttpEvent, HttpHandler, HttpInterceptor, HttpRequest, HttpResponse} from '@angular/common/http';
import {SessionStorageService} from './services/session-storage.service';
import {Observable, of} from 'rxjs';
import {catchError, finalize, map, tap} from 'rxjs/operators';
import {ErrorHandlingService} from './shared/services/error-handling.service';

@Injectable({
  providedIn: 'root'
})
export class Interceptor  implements HttpInterceptor {

  constructor(private sessionStorageService: SessionStorageService,
              private errorHandlingService: ErrorHandlingService) { }

  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {

    if (this.sessionStorageService.isUserLoggedIn()) {
      request = request.clone({
        setHeaders: {
          Authorization: this.sessionStorageService.getToken()
        }
      });
    }
    return next.handle(request).pipe(
      tap(
        result => this.handleValidResponse(result),
        error => this.handleErrorResponse(error))
    );
  }
  handleValidResponse(result: HttpEvent<any>): void {
    if (result instanceof HttpResponse) {
      const response = result as HttpResponse<any>;
      if (response.headers.get('Authorization') !== null && response.headers.get('X-User') !== null) {
        const userJson = response.headers.get('X-User');
        const user = JSON.parse(userJson);
        this.sessionStorageService.storeSession(user, response.headers.get('Authorization'));
        console.log(this.sessionStorageService.getUser().username)
      }
    }
  }
  handleErrorResponse(error: any): Observable<any> {
    this.errorHandlingService.handle(error);
    return of(error);
  }
}
