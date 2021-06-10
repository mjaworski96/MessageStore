import { Injectable } from '@angular/core';
import {AliasWithIdList} from '../model/alias';
import {AliasService} from './alias.service';
import {ActivatedRouteSnapshot, Resolve, RouterStateSnapshot} from '@angular/router';
import {Observable} from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AliasInternalOnlyResolveService implements Resolve<AliasWithIdList> {
  constructor(private aliasService: AliasService) { }

  resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<AliasWithIdList> | Promise<AliasWithIdList> | AliasWithIdList {
    return this.aliasService.getAll(true);
  }
}
