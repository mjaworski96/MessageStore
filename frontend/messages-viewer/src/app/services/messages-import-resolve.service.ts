import { Injectable } from '@angular/core';
import {ImportService} from './import.service';
import {ActivatedRouteSnapshot, RouterStateSnapshot} from '@angular/router';
import {Observable} from 'rxjs';
import {ImportsList} from '../model/import';
import {MessagesImportService} from './messages-import.service';
import {MessagesImportList} from '../model/message-import';

@Injectable({
  providedIn: 'root'
})
export class MessagesImportResolveService {

  constructor(private messagesImportService: MessagesImportService) { }

  resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot):
    Observable<MessagesImportList> | Promise<MessagesImportList> | MessagesImportList {
    return this.messagesImportService.getAll();
  }
}
