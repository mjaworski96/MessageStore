import { Component, OnInit } from '@angular/core';
import {ImportsList} from '../../model/import';
import {ActivatedRoute} from '@angular/router';
import {ImportService} from '../../services/import.service';
import {MessagesImportService} from '../../services/messages-import.service';
import {MessagesImportList} from '../../model/message-import';
import {MatDialog} from '@angular/material';
import {DeleteMessagesDialogComponent} from './delete-messages-dialog/delete-messages-dialog.component';

@Component({
  selector: 'app-import-view',
  templateUrl: './import-view.component.html',
  styleUrls: ['./import-view.component.css']
})
export class ImportViewComponent implements OnInit {

  imports: MessagesImportList;
  deleting = false;
  constructor(private route: ActivatedRoute,
              private messagesImportService: MessagesImportService,
              private dialog: MatDialog) { }

  ngOnInit() {
    this.imports = this.route.snapshot.data.imports;
  }
  async delete(importId: string) {
    const dialogRef = this.dialog.open(DeleteMessagesDialogComponent, {
      height: '250px',
      width: '350px',
    });

    try {
      const res = await dialogRef.afterClosed().toPromise();
      if (res === true) {
        this.deleting = true;
        await this.messagesImportService.delete(importId);
        this.imports.imports = this.imports.imports.filter(x => x.id !== importId);
      }
    } finally {
      this.deleting = false;
    }
  }
}
