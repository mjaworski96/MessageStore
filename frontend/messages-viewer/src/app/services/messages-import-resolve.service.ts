import { Injectable } from '@angular/core';
import {ActivatedRouteSnapshot, Resolve, RouterStateSnapshot} from '@angular/router';
import {Observable} from 'rxjs';
import {MessagesImportService} from './messages-import.service';
import {MessagesImportList} from '../model/message-import';

@Injectable({
  providedIn: 'root'
})
export class MessagesImportResolveService implements Resolve<MessagesImportList> {

  constructor(private messagesImportService: MessagesImportService) { }

  resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot):
    Observable<MessagesImportList> | Promise<MessagesImportList> | MessagesImportList {
    return this.messagesImportService.getAll();
  }
}
