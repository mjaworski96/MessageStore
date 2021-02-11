import { Component, OnInit } from '@angular/core';
import {UserAccountService} from '../../../services/user-account.service';
import {SessionStorageService} from '../../../services/session-storage.service';
import {Router} from '@angular/router';
import {MatDialog} from '@angular/material';
import {DeleteUserAccountDialogComponent} from './delete-user-account-dialog/delete-user-account-dialog.component';

@Component({
  selector: 'app-delete-account',
  templateUrl: './delete-account.component.html',
  styleUrls: ['./delete-account.component.css']
})
export class DeleteAccountComponent implements OnInit {

  constructor(private userAccountService: UserAccountService,
              private sessionStorageService: SessionStorageService,
              private router: Router,
              private dialog: MatDialog) { }

  ngOnInit(): void {
  }

  delete(): void {
    const dialogRef = this.dialog.open(DeleteUserAccountDialogComponent, {
      height: '250px',
      width: '350px',
    });
    dialogRef.afterClosed().toPromise().then( res => {
      if (res === true) {
        this.userAccountService.deleteAccount()
          .toPromise().then(result => {
          this.sessionStorageService.logout();
          this.router.navigate(['/']);
        });
      }
    });
  }

}
