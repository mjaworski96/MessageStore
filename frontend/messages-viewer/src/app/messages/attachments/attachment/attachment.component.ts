import {Component, ElementRef, Input, OnInit, ViewChild} from '@angular/core';
import {AttachmentWithID} from '../../../model/attachment';
import {SessionStorageService} from '../../../services/session-storage.service';
import {AttachmentService} from '../../../services/attachment.service';

@Component({
  selector: 'app-attachment',
  templateUrl: './attachment.component.html',
  styleUrls: ['./attachment.component.css']
})
export class AttachmentComponent implements OnInit {
  @Input()
  attachment: AttachmentWithID;

  constructor(private sessionStorageService: SessionStorageService,
              private attachmentService: AttachmentService) { }

  ngOnInit() {
  }
  getVideoUrl(): string {
    return `/api/attachments/${this.attachment.id}/stream?access_token=${this.sessionStorageService.getToken()}`;
  }
  getType() {
    return this.attachment.contentType.split('/')[0];
  }
  downloadAttachment() {
    this.attachmentService.downloadAttachment(this.attachment.id, this.attachment.saveAsFilename);
  }
}
