import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-messenger-import-info',
  templateUrl: './messenger-import-info.component.html',
  styleUrls: ['./messenger-import-info.component.css']
})
export class MessengerImportInfoComponent implements OnInit {
  expand = false;
  constructor() { }

  ngOnInit() {
  }
  changeExpand() {
    this.expand = !this.expand;
  }
}
