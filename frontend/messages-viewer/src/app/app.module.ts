import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import {CoreModule} from './core/core.module';
import {Interceptor} from './interceptor';
import {HTTP_INTERCEPTORS} from '@angular/common/http';
import {AliasesModule} from './aliases/aliases.module';
import {MessagesModule} from './messages/messages.module';
import { NotFoundComponent } from './not-found/not-found.component';
import {ToastrModule} from 'ngx-toastr';
import {AuthorizationModule} from './authorization/authorization.module';
import {UserEditModule} from './user-edit/user-edit.module';
import {MessengerModule} from './messenger/messenger.module';
import {ImportsModule} from './imports/imports.module';
import { MainPageComponent } from './main-page/main-page.component';
import {MatButtonModule} from '@angular/material';

@NgModule({
  declarations: [
    AppComponent,
    NotFoundComponent,
    MainPageComponent
  ],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    CoreModule,
    AliasesModule,
    MessagesModule,
    MessengerModule,
    AuthorizationModule,
    UserEditModule,
    ImportsModule,
    AppRoutingModule,
    MatButtonModule,
    ToastrModule.forRoot(),
  ],
  providers: [
    {
      provide: HTTP_INTERCEPTORS,
      useClass: Interceptor,
      multi: true
    },
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
