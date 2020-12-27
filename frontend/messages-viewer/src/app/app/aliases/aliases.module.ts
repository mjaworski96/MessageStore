import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AliasesRoutingModule } from './aliases-routing.module';
import { AliasViewComponent } from './alias-view/alias-view.component';
import { AliasEditorComponent } from './alias-editor/alias-editor.component';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';
import {MatButtonModule, MatCheckboxModule, MatDialogModule, MatFormFieldModule, MatInputModule} from '@angular/material';
import {BrowserAnimationsModule} from '@angular/platform-browser/animations';
import { DeleteAliasDialogComponent } from './alias-view/delete-alias-dialog/delete-alias-dialog.component';


@NgModule({
  declarations: [AliasViewComponent, AliasEditorComponent, DeleteAliasDialogComponent],
  imports: [
    CommonModule,
    AliasesRoutingModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    FormsModule,
    MatButtonModule,
    BrowserAnimationsModule,
    MatCheckboxModule,
    MatDialogModule
  ],
  entryComponents: [
    DeleteAliasDialogComponent
  ]
})
export class AliasesModule { }
