import {UserAccountResolveService} from '../services/user-account-resolve.service';
import {Route, RouterModule} from '@angular/router';
import {UserEditPageComponent} from './user-edit-page/user-edit-page.component';
import {NgModule} from '@angular/core';
import {LoggedUserGuard} from '../guards/logged-user-guard';

const USER_EDIT_PAGE_ROUTES: Route[] = [
  {
    path: 'account',
    component: UserEditPageComponent,
    resolve: {
      user: UserAccountResolveService
    },
    canActivate: [LoggedUserGuard],
    runGuardsAndResolvers: 'always'
  },
];

@NgModule({
  imports: [
    RouterModule.forChild(USER_EDIT_PAGE_ROUTES),
  ],
  exports: [
    RouterModule
  ]
})
export class UserEditRoutingModule {}

