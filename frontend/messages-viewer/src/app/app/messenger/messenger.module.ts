import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MessengerPageComponent } from './messenger-page/messenger-page.component';
import { MessengerImportComponent } from './messenger-page/messenger-import/messenger-import.component';
import { MessengerImportsListComponent } from './messenger-page/messenger-imports-list/messenger-imports-list.component';
import {MessengerRoutingModule} from './messenger-routing.module';
import { MessengerImportInfoComponent } from './messenger-page/messenger-import/messenger-import-info/messenger-import-info.component';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';
import {MatButtonModule, MatFormFieldModule, MatInputModule, MatProgressBarModule} from '@angular/material';
import {BrowserAnimationsModule} from '@angular/platform-browser/animations';



@NgModule({
  declarations: [MessengerPageComponent, MessengerImportComponent, MessengerImportsListComponent, MessengerImportInfoComponent],
  imports: [
    CommonModule,
    MessengerRoutingModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    FormsModule,
    MatButtonModule,
    BrowserAnimationsModule,
    MatProgressBarModule
  ]
})
export class MessengerModule { }
