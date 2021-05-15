import {Component, Input, OnInit} from '@angular/core';

@Component({
  selector: 'app-application-name',
  templateUrl: './application-name.component.html',
  styleUrls: ['./application-name.component.css']
})
export class ApplicationNameComponent implements OnInit {

  @Input()
  application: string;
  constructor() { }

  ngOnInit() {
  }

}
