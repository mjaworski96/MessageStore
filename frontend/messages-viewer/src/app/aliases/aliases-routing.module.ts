import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import {AliasViewComponent} from './alias-view/alias-view.component';
import {LoggedUserGuard} from '../guards/logged-user-guard';
import {AliasEditorComponent} from './alias-editor/alias-editor.component';


const routes: Routes = [
  {
    path: 'aliases',
    component: AliasViewComponent,
    canActivate: [LoggedUserGuard],
  },
  {
    path: 'aliases/new',
    component: AliasEditorComponent,
    canActivate: [LoggedUserGuard],
  },
  {
    path: 'aliases/edit/:id',
    component: AliasEditorComponent,
    canActivate: [LoggedUserGuard],
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AliasesRoutingModule { }
