import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import {SafeImagePipe} from './pipes/safe-image.pipe';



@NgModule({
  declarations: [SafeImagePipe],
  imports: [
    CommonModule
  ],
  exports: [
    SafeImagePipe
  ]
})
export class SharedModule { }
