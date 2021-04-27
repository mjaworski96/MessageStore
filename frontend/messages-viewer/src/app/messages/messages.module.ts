import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { MessagesRoutingModule } from './messages-routing.module';
import { MessagesListComponent } from './messages-list/messages-list.component';
import { SearchComponent } from './search/search.component';
import {MatButtonModule, MatCheckboxModule, MatInputModule} from '@angular/material';
import {FormsModule} from '@angular/forms';
import {SharedModule} from '../shared/shared.module';
import { AttachmentsComponent } from './attachments/attachments.component';
import { AttachmentComponent } from './attachments/attachment/attachment.component';


@NgModule({
  declarations: [MessagesListComponent, SearchComponent, AttachmentsComponent, AttachmentComponent],
  imports: [
    CommonModule,
    MessagesRoutingModule,
    MatInputModule,
    MatCheckboxModule,
    FormsModule,
    MatInputModule,
    MatButtonModule,
    SharedModule
  ]
})
export class MessagesModule { }
