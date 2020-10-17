import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import {HttpClientModule} from '@angular/common/http';
import {NotFoundComponent} from './not-found/not-found.component';


const routes: Routes = [
  {
    path: 'not-found',
    component: NotFoundComponent
  },
  // TODO: Add main page
  // {
  //   path: '**',
  //   redirectTo: 'not-found'
  // },
];

@NgModule({
  imports: [
    RouterModule.forRoot(routes),
    HttpClientModule
  ],
  exports: [RouterModule]
})
export class AppRoutingModule { }
