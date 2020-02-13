import { Component, OnInit } from '@angular/core';
import {AliasService} from '../../../services/alias.service';
import {AliasWithId} from '../../../model/alias';
import {Router} from '@angular/router';

@Component({
  selector: 'app-alias-view',
  templateUrl: './alias-view.component.html',
  styleUrls: ['./alias-view.component.css']
})
export class AliasViewComponent implements OnInit {

  aliases: AliasWithId[];

  constructor(private aliasService: AliasService,
              private router: Router) { }

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
}
