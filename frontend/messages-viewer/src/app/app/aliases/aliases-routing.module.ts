import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import {AliasViewComponent} from './alias-view/alias-view.component';
import {LoggedUserGuard} from '../shared/guards/logged-user-guard';


const routes: Routes = [
  {
    path: 'aliases',
    component: AliasViewComponent,
    canActivate: [LoggedUserGuard],
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AliasesRoutingModule { }
