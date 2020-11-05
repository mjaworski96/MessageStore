import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import {CoreModule} from './app/core/core.module';
import {Interceptor} from './interceptor';
import {HTTP_INTERCEPTORS} from '@angular/common/http';
import {AliasesModule} from './app/aliases/aliases.module';
import {MessagesModule} from './app/messages/messages.module';
import { NotFoundComponent } from './not-found/not-found.component';
import {ToastrModule} from 'ngx-toastr';
import {AuthorizationModule} from './app/authorization/authorization.module';
import {UserEditModule} from './app/user-edit/user-edit.module';

@NgModule({
  declarations: [
    AppComponent,
    NotFoundComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    BrowserAnimationsModule,
    CoreModule,
    AliasesModule,
    MessagesModule,
    AuthorizationModule,
    UserEditModule,
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
