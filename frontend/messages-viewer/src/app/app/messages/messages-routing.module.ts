import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import {MessagesListComponent} from './messages-list/messages-list.component';
import {SearchComponent} from './search/search.component';


const routes: Routes = [
  {
    path: 'messages',
    component: MessagesListComponent
  },
  {
    path: 'search',
    component: SearchComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class MessagesRoutingModule { }
