import { Injectable } from '@angular/core';
import {AliasWithId, AliasWithIdList} from '../model/alias';
import {AliasService} from './alias.service';
import {ActivatedRouteSnapshot, Resolve, RouterStateSnapshot} from '@angular/router';
import {Observable} from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class SingleAliasResolveService implements Resolve<AliasWithId> {
  constructor(private aliasService: AliasService) { }

  resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<AliasWithId> | Promise<AliasWithId> | AliasWithId {
    const id = +route.paramMap.get('id');
    if (id) {
      return this.aliasService.get(id).toPromise();
    }
    return null;
  }
}
