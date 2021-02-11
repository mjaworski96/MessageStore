import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { LoggedUserGuard } from '../shared/guards/logged-user-guard';
import { MessengerPageComponent } from './messenger-page/messenger-page.component';
import {ImportResolveService} from '../../services/import-resolve.service';

const routes: Routes = [
  {
    path: 'messenger',
    component: MessengerPageComponent,
    canActivate: [LoggedUserGuard],
    resolve: {
      imports: ImportResolveService
    },
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class MessengerRoutingModule { }
