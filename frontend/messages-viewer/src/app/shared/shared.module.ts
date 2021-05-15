import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import {SafeImagePipe} from './pipes/safe-image.pipe';
import { AttachmentPipe } from './pipes/attachment.pipe';
import { NotNullPipe } from './pipes/not-null.pipe';
import { ApplicationNameComponent } from './components/application-name/application-name.component';



@NgModule({
  declarations: [SafeImagePipe, AttachmentPipe, NotNullPipe, ApplicationNameComponent],
  imports: [
    CommonModule
  ],
  exports: [
    SafeImagePipe,
    AttachmentPipe,
    NotNullPipe,
    ApplicationNameComponent
  ]
})
export class SharedModule { }
