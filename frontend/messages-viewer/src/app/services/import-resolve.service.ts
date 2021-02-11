import { Injectable } from '@angular/core';
import {ActivatedRouteSnapshot, Resolve, RouterStateSnapshot} from '@angular/router';
import {ImportsList} from '../model/import';
import {Observable} from 'rxjs';
import {ImportService} from './import.service';
@Injectable({
  providedIn: 'root'
})
export class ImportResolveService implements Resolve<ImportsList> {

  constructor(private importService: ImportService) { }

  resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot):
    Observable<ImportsList> | Promise<ImportsList> | ImportsList {
    return this.importService.getAll();
  }
}
