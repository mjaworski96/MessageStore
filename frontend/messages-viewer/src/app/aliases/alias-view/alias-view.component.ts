import { Component, OnInit } from '@angular/core';
import {AliasService} from '../../services/alias.service';
import {AliasWithId, AliasWithIdList} from '../../model/alias';
import {ActivatedRoute, Router} from '@angular/router';
import {MatDialog} from '@angular/material';
import {DeleteAliasDialogComponent} from './delete-alias-dialog/delete-alias-dialog.component';
import {DialogConfig} from '../../shared/utils/dialog-config';
import {EditAliasNameDialogComponent} from './edit-alias-name-dialog/edit-alias-name-dialog.component';

@Component({
  selector: 'app-alias-view',
  templateUrl: './alias-view.component.html',
  styleUrls: ['./alias-view.component.css']
})
export class AliasViewComponent implements OnInit {

  aliases: AliasWithId[];
  filtered: AliasWithId[];
  filterBy = '';
  constructor(private aliasService: AliasService,
              private router: Router,
              private route: ActivatedRoute,
              private dialog: MatDialog) { }

  ngOnInit() {
    const aliases: AliasWithIdList = this.route.snapshot.data.aliases;
    this.aliases = aliases.aliases;
    this.filtered = this.aliases;
  }
  filterByChange(event) {
    this.filter();
  }
  filter() {
    if (this.filterBy) {
      const filterByLowerCase = this.filterBy.toLowerCase();
      this.filtered = this.aliases.filter(x => x.name.toLowerCase().includes(filterByLowerCase));
    } else {
      this.filtered = this.aliases;
    }
  }
  navigate(alias: AliasWithId, event): void {
    console.log(event);
    if (event.ctrlKey) {
      window.open(this.router.serializeUrl(
        this.router.createUrlTree(['messages'], {
          queryParams: {
            aliasId: alias.id,
            page: 1
          }
        })));
    } else {
      this.router.navigate(['messages'], {
        queryParams: {
          aliasId: alias.id,
          page: 1
        }
      });
    }

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
    const dialogRef = this.dialog.open(DeleteAliasDialogComponent, DialogConfig);
    dialogRef.afterClosed().toPromise().then( res => {
      if (res === true) {
        this.aliasService.delete(id)
          .toPromise()
          .then(() => {
            this.aliases = this.aliases.filter(x => x.id !== id);
          });
      }
    });
  }
  async editName(alias: AliasWithId, event: Event) {
    event.stopImmediatePropagation();
    const config: any = DialogConfig;
    config.data =  {
      alias,
      aliasService: this.aliasService
    };
    const dialogRef = this.dialog.open(EditAliasNameDialogComponent, DialogConfig);
    const name = await dialogRef.afterClosed().toPromise();
    if (name) {
      alias.name = name;
    }
  }
}
