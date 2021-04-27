import {Component, ElementRef, Input, OnInit, ViewChild} from '@angular/core';
import {AttachmentWithID} from '../../../model/attachment';
import {SessionStorageService} from '../../../services/session-storage.service';

@Component({
  selector: 'app-attachment',
  templateUrl: './attachment.component.html',
  styleUrls: ['./attachment.component.css']
})
export class AttachmentComponent implements OnInit {
  apiAttachmentsUrl = '/api/attachments/';
  streamUrl = '/stream';

  @Input()
  attachment: AttachmentWithID;

  constructor(private sessionStorageService: SessionStorageService) { }

  ngOnInit() {
  }
  getVideoUrl(): string {
    return `${this.apiAttachmentsUrl}${this.attachment.id}${this.streamUrl}?access_token=${this.sessionStorageService.getToken()}`;
  }
  getType() {
    return this.attachment.contentType.split('/')[0];
  }
}
