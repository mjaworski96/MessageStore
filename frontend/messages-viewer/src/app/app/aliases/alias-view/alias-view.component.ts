import { Component, OnInit } from '@angular/core';
import {AliasService} from '../../../services/alias.service';
import {AliasWithIdList} from '../../model/alias';

@Component({
  selector: 'app-alias-view',
  templateUrl: './alias-view.component.html',
  styleUrls: ['./alias-view.component.css']
})
export class AliasViewComponent implements OnInit {

  aliases: AliasWithIdList;

  constructor(private aliasService: AliasService) { }

  ngOnInit() {
    this.aliasService
      .getAll()
      .toPromise()
      .then((result) => {
        this.aliases = result;
      });
  }

}
