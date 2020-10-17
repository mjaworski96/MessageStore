import {Injectable} from '@angular/core';
import {HttpEvent, HttpHandler, HttpInterceptor, HttpRequest, HttpResponse} from '@angular/common/http';
import {SessionStorageService} from './services/session-storage.service';
import {Observable} from 'rxjs';
import {finalize, tap} from 'rxjs/operators';
import {ErrorHandlingService} from './shared/services/error-handling.service';

@Injectable({
  providedIn: 'root'
})
export class Interceptor  implements HttpInterceptor {

  constructor(private sessionStorageService: SessionStorageService,
              private errorHandlingService: ErrorHandlingService) { }

  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    request = request.clone({
      setHeaders: {
        'X-MockedAuthority': this.sessionStorageService.getUsername()
      }
    });
    return next.handle(request).pipe(
      tap(
        result => this.handleValidResponse(result),
        error => this.errorHandlingService.handle(error)));
  }
  handleValidResponse(result: HttpEvent<any>): void {
    // TODO: Authorization
  }
}
