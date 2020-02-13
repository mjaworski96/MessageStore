import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import {AliasViewComponent} from './alias-view/alias-view.component';


const routes: Routes = [
  {
    path: 'aliases',
    component: AliasViewComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AliasesRoutingModule { }
