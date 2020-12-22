import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AliasesRoutingModule } from './aliases-routing.module';
import { AliasViewComponent } from './alias-view/alias-view.component';
import { AliasEditorComponent } from './alias-editor/alias-editor.component';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';
import {MatButtonModule, MatCheckboxModule, MatFormFieldModule, MatInputModule} from '@angular/material';
import {BrowserAnimationsModule} from '@angular/platform-browser/animations';


@NgModule({
  declarations: [AliasViewComponent, AliasEditorComponent],
  imports: [
    CommonModule,
    AliasesRoutingModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    FormsModule,
    MatButtonModule,
    BrowserAnimationsModule,
    MatCheckboxModule
  ]
})
export class AliasesModule { }
