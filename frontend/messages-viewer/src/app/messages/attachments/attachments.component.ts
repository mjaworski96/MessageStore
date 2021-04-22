import {Component, Input, OnInit} from '@angular/core';
import {AttachmentWithID} from '../../model/attachment';

@Component({
  selector: 'app-attachments',
  templateUrl: './attachments.component.html',
  styleUrls: ['./attachments.component.css']
})
export class AttachmentsComponent implements OnInit {

  apiAttachmentsUrl = '/api/attachments/';

  @Input()
  attachments: AttachmentWithID[];
  constructor() { }

  ngOnInit() {
  }
  getType(attachment: AttachmentWithID) {
    return attachment.contentType.split('/')[0];
  }
}
