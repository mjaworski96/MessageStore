import {Injectable} from '@angular/core';
import {HttpEvent, HttpHandler, HttpInterceptor, HttpRequest} from '@angular/common/http';
import {SessionStorageService} from './services/session-storage.service';
import {Observable} from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class Interceptor  implements HttpInterceptor {

  constructor(private sessionStorageService: SessionStorageService) { }

  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    request = request.clone({
      setHeaders: {
        'X-MockedAuthority': this.sessionStorageService.getUsername()
      }
    });
    return next.handle(request);
  }
}
