import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import {MessagesListComponent} from './messages-list/messages-list.component';
import {SearchComponent} from './search/search.component';
import {LoggedUserGuard} from '../guards/logged-user-guard';


const routes: Routes = [
  {
    path: 'messages',
    component: MessagesListComponent,
    canActivate: [LoggedUserGuard],
  },
  {
    path: 'search',
    component: SearchComponent,
    canActivate: [LoggedUserGuard],
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class MessagesRoutingModule { }
