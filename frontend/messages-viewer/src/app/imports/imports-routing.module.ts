import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import {LoggedUserGuard} from '../guards/logged-user-guard';
import {ImportViewComponent} from './import-view/import-view.component';
import {MessagesImportResolveService} from '../services/messages-import-resolve.service';

const routes: Routes = [
  {
    path: 'imports',
    component: ImportViewComponent,
    canActivate: [LoggedUserGuard],
    resolve: {
      imports: MessagesImportResolveService
    },
    runGuardsAndResolvers: 'always'
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ImportsRoutingModule { }
