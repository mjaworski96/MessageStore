import { Pipe, PipeTransform } from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {Observable} from 'rxjs';
import { switchMap } from 'rxjs/operators';
@Pipe({
  name: 'attachment'
})
export class AttachmentPipe implements PipeTransform {
  constructor(private http: HttpClient) {}

  transform(url: any, ...args: any[]): any {
    if (!url) {
      return '';
    }
    return new Observable<string>((observer) => {
      this.http.get(url, {responseType: 'blob'}).subscribe(response => {
        const reader = new FileReader();
        reader.readAsDataURL(response);
        reader.onloadend = () => {
          observer.next(reader.result.toString());
        };
      }, err => {
        observer.next('');
      });

      return {unsubscribe() { }};
    });
  }

}
