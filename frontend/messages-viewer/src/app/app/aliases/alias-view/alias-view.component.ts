import { Component, OnInit } from '@angular/core';
import {AliasService} from '../../../services/alias.service';
import {AliasWithId} from '../../../model/alias';
import {Router} from '@angular/router';
import {MatDialog} from '@angular/material';
import {DeleteUserAccountDialogComponent} from '../../user-edit/user-edit-page/delete-account/delete-user-account-dialog/delete-user-account-dialog.component';
import {DeleteAliasDialogComponent} from './delete-alias-dialog/delete-alias-dialog.component';

@Component({
  selector: 'app-alias-view',
  templateUrl: './alias-view.component.html',
  styleUrls: ['./alias-view.component.css']
})
export class AliasViewComponent implements OnInit {

  aliases: AliasWithId[];

  constructor(private aliasService: AliasService,
              private router: Router,
              private dialog: MatDialog) { }

  ngOnInit() {
    this.aliasService
      .getAll()
      .toPromise()
      .then((result) => {
        this.aliases = result.aliases;
      });
  }
  navigate(alias: AliasWithId): void {
    this.router.navigate(['messages'], {
      queryParams: {
        aliasId: alias.id,
        page: 1
      }
    });
  }
  addNew() {
    this.router.navigate(['aliases', 'new']);
  }
  edit(id: number, event: Event) {
    event.stopImmediatePropagation();
    this.router.navigate(['aliases', 'edit', id]);
  }
  delete(id: number, event: Event) {
    event.stopImmediatePropagation();
    const dialogRef = this.dialog.open(DeleteAliasDialogComponent, {
      height: '250px',
      width: '350px',
    });
    dialogRef.afterClosed().toPromise().then( res => {
      if (res === true) {
        console.log('confirm')
        this.aliasService.delete(id)
          .toPromise()
          .then(() => {
            this.aliases = this.aliases.filter(x => x.id !== id);
          });
      }
    });
  }
}
