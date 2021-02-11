import {Component, Input, OnInit} from '@angular/core';
import {ImportsList} from '../../../../model/import';

@Component({
  selector: 'app-messenger-imports-list',
  templateUrl: './messenger-imports-list.component.html',
  styleUrls: ['./messenger-imports-list.component.css']
})
export class MessengerImportsListComponent implements OnInit {
  @Input()
  imports: ImportsList;

  constructor() { }

  ngOnInit() {
  }

}
