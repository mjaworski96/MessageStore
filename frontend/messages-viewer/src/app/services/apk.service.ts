import { Injectable } from '@angular/core';
import {HttpClient} from '@angular/common/http';
import { saveAs } from 'file-saver';

@Injectable({
  providedIn: 'root'
})
export class ApkService {

  url = '/api/aliases';

  constructor(private http: HttpClient) { }

  downloadApk(): Promise<any> {
    return this.http.get(`/api/apk`, {responseType: 'blob'})
      .toPromise()
      .then(result => {
        saveAs(result, 'com.companyname.messagesender-Signed.apk');
      });
  }
}
