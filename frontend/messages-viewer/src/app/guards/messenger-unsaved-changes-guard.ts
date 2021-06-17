import {ActivatedRouteSnapshot, CanDeactivate, RouterStateSnapshot, UrlTree} from '@angular/router';
import {Injectable} from '@angular/core';
import {MatDialog} from '@angular/material';
import {MessengerImportComponent} from '../messenger/messenger-page/messenger-import/messenger-import.component';
import {Observable} from 'rxjs';
import {DeleteAliasDialogComponent} from '../aliases/alias-view/delete-alias-dialog/delete-alias-dialog.component';
import {DialogConfig} from '../shared/utils/dialog-config';
import {MessengerLeavePageDialogComponent} from '../messenger/messenger-page/messenger-import/messenger-leave-page-dialog/messenger-leave-page-dialog.component';
import {MessengerPageComponent} from '../messenger/messenger-page/messenger-page.component';

@Injectable({
  providedIn: 'root'
})
export class MessengerUnsavedChangesGuard implements CanDeactivate<MessengerPageComponent> {
  constructor(private dialog: MatDialog) {}

  async canDeactivate(component: MessengerPageComponent, currentRoute: ActivatedRouteSnapshot,
                      currentState: RouterStateSnapshot, nextState?: RouterStateSnapshot):
    Promise<boolean | UrlTree> {
    if (component.importProgress.importInProgress) {
      const dialogRef = this.dialog.open(MessengerLeavePageDialogComponent, DialogConfig);
      const result = await dialogRef.afterClosed().toPromise();

      if (result) {
        component.importProgress.importCanceled = true;
      }

      return result;
    }
    return true;
  }
}
