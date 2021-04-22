import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import {SafeImagePipe} from './pipes/safe-image.pipe';
import { AttachmentPipe } from './pipes/attachment.pipe';
import { NotNullPipe } from './pipes/not-null.pipe';



@NgModule({
  declarations: [SafeImagePipe, AttachmentPipe, NotNullPipe],
  imports: [
    CommonModule
  ],
  exports: [
    SafeImagePipe,
    AttachmentPipe,
    NotNullPipe
  ]
})
export class SharedModule { }
