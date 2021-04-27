import {Component, Input, OnInit, ViewChild} from '@angular/core';
import {AttachmentWithID} from '../../model/attachment';

@Component({
  selector: 'app-attachments',
  templateUrl: './attachments.component.html',
  styleUrls: ['./attachments.component.css']
})
export class AttachmentsComponent implements OnInit {
  @Input()
  attachments: AttachmentWithID[];

  constructor() { }

  ngOnInit() {
  }
}
