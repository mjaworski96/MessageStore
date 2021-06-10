import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import {AliasViewComponent} from './alias-view/alias-view.component';
import {LoggedUserGuard} from '../guards/logged-user-guard';
import {AliasEditorComponent} from './alias-editor/alias-editor.component';
import {AliasResolveService} from '../services/alias-resolve.service';
import {AliasInternalOnlyResolveService} from '../services/alias-internal-only-resolve.service';
import {SingleAliasResolveService} from '../services/single-alias-resolve.service';


const routes: Routes = [
  {
    path: 'aliases',
    component: AliasViewComponent,
    canActivate: [LoggedUserGuard],
    runGuardsAndResolvers: 'always',
    resolve: {
      aliases: AliasResolveService
    },
  },
  {
    path: 'aliases/new',
    component: AliasEditorComponent,
    canActivate: [LoggedUserGuard],
    runGuardsAndResolvers: 'always',
    resolve: {
      aliases: AliasInternalOnlyResolveService
    },
  },
  {
    path: 'aliases/edit/:id',
    component: AliasEditorComponent,
    canActivate: [LoggedUserGuard],
    runGuardsAndResolvers: 'always',
    resolve: {
      aliases: AliasInternalOnlyResolveService,
      originalAlias: SingleAliasResolveService
    },
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AliasesRoutingModule { }
