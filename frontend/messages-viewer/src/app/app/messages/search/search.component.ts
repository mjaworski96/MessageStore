import { Component, OnInit } from '@angular/core';
import {AliasService} from '../../../services/alias.service';
import {SearchService} from '../../../services/search.service';
import {SearchResultDto} from '../../../model/search';
import {Router} from '@angular/router';
import {MessagesListComponent} from '../messages-list/messages-list.component';

interface Checkbox {
  aliasId: number;
  checked: boolean;
  name: string;
}

@Component({
  selector: 'app-search',
  templateUrl: './search.component.html',
  styleUrls: ['./search.component.css']
})
export class SearchComponent implements OnInit {

  query = '';
  aliases: Checkbox[];
  ignoreLetterSize = true;

  results: SearchResultDto[];

  constructor(private aliasService: AliasService,
              private searchService: SearchService,
              private router: Router) { }

  ngOnInit() {
    this.getAliases();
  }
  getAliases(): void {
    this.aliases = [];
    this.aliasService.getAll()
      .toPromise()
      .then(result => {
        result.aliases.forEach(item => {
          this.aliases.push({
            aliasId: item.id,
            checked: false,
            name: item.name
          });
        });
      });
  }
  getAliasesIds(): number[] {
    const ids = [];
    for (let i = 0; i < this.aliases.length; i++) {
      const item = this.aliases[i];
      if (item.checked) {
        ids.push(item.aliasId);
      }
    }
    return ids;
  }
  search() {
    this.results = [];
    this.searchService.search(
      {
        aliasesIds: this.getAliasesIds(),
        ignoreLetterSize: this.ignoreLetterSize,
        query: this.query
      }
    )
      .toPromise()
      .then(res => {
        this.results = res.results;
      });
  }
  navigate(result: SearchResultDto): void {
    this.searchService
      .getOrder(result.messageId, result.aliasId)
      .toPromise()
      .then(order => {
        const messageOnPage = Math.ceil(order.order / MessagesListComponent.pageSize);
        this.router.navigate(['messages'], {
          queryParams: {
            aliasId: result.aliasId,
            page: messageOnPage
          }
        });
      });
  }
  writtenByAppUser(result: SearchResultDto): boolean {
    return result.writerType === 'app_user';
  }
  writtenByContact(result: SearchResultDto): boolean {
    return result.writerType === 'contact';
  }
}
