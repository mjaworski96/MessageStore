import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import {MessagesListComponent} from './messages-list/messages-list.component';
import {SearchComponent} from './search/search.component';
import {LoggedUserGuard} from '../guards/logged-user-guard';
import {AliasResolveService} from '../services/alias-resolve.service';


const routes: Routes = [
  {
    path: 'messages',
    component: MessagesListComponent,
    canActivate: [LoggedUserGuard],
    runGuardsAndResolvers: 'always'
  },
  {
    path: 'search',
    component: SearchComponent,
    resolve: {
      aliases: AliasResolveService
    },
    canActivate: [LoggedUserGuard],
    runGuardsAndResolvers: 'always'
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class MessagesRoutingModule { }
