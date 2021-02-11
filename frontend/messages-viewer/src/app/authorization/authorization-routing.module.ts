import {Route, RouterModule} from '@angular/router';
import {RegistrationPageComponent} from './registration-page/registration-page.component';
import {LoginPageComponent} from './login-page/login-page.component';
import {NgModule} from '@angular/core';
import {NotLoggedUserGuard} from '../shared/guards/not-logged-user-guard';

const AUTHENTICATION_PAGE_ROUTES: Route[] = [
  {
    path: 'register',
    component: RegistrationPageComponent,
    canActivate: [NotLoggedUserGuard],
    runGuardsAndResolvers: 'always'
  },
  {
    path: 'login',
    component: LoginPageComponent,
    canActivate: [NotLoggedUserGuard],
    runGuardsAndResolvers: 'always'
  }
];

@NgModule({
  imports: [
    RouterModule.forChild(AUTHENTICATION_PAGE_ROUTES),
  ],
  exports: [
    RouterModule
  ]
})
export class AuthorizationRoutingModule {

}
