import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { LoggedUserGuard } from '../guards/logged-user-guard';
import { MessengerPageComponent } from './messenger-page/messenger-page.component';
import {ImportResolveService} from '../services/import-resolve.service';
import {MessengerUnsavedChangesGuard} from '../guards/messenger-unsaved-changes-guard';

const routes: Routes = [
  {
    path: 'messenger',
    component: MessengerPageComponent,
    canActivate: [LoggedUserGuard],
    canDeactivate: [MessengerUnsavedChangesGuard],
    resolve: {
      imports: ImportResolveService
    },
    runGuardsAndResolvers: 'always'
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class MessengerRoutingModule { }
