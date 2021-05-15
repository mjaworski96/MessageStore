import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ImportViewComponent } from './import-view/import-view.component';
import {ImportsRoutingModule} from './imports-routing.module';
import {SharedModule} from '../shared/shared.module';
import { DeleteMessagesDialogComponent } from './import-view/delete-messages-dialog/delete-messages-dialog.component';
import {MatButtonModule, MatDialogModule} from '@angular/material';



@NgModule({
  declarations: [ImportViewComponent, DeleteMessagesDialogComponent],
  imports: [
    CommonModule,
    ImportsRoutingModule,
    SharedModule,
    MatDialogModule,
    MatButtonModule
  ],
  entryComponents: [
    DeleteMessagesDialogComponent
  ]
})
export class ImportsModule { }
