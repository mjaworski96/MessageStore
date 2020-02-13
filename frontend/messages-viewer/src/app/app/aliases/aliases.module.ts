import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { AliasesRoutingModule } from './aliases-routing.module';
import { AliasViewComponent } from './alias-view/alias-view.component';


@NgModule({
  declarations: [AliasViewComponent],
  imports: [
    CommonModule,
    AliasesRoutingModule
  ]
})
export class AliasesModule { }
