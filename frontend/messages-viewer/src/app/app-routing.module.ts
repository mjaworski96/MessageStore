import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import {AliasesModule} from './app/aliases/aliases.module';
import {HttpClientModule} from '@angular/common/http';


const routes: Routes = [];

@NgModule({
  imports: [
    RouterModule.forRoot(routes),
    AliasesModule,
    HttpClientModule
  ],
  exports: [RouterModule]
})
export class AppRoutingModule { }
