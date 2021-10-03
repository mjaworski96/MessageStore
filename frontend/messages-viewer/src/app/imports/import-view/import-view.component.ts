import { Component, OnDestroy, OnInit } from '@angular/core';
import {ImportsList} from '../../model/import';
import {ActivatedRoute} from '@angular/router';
import {ImportService} from '../../services/import.service';
import {MessagesImportService} from '../../services/messages-import.service';
import {MessagesImportList} from '../../model/message-import';
import {MatDialog} from '@angular/material';
import {DeleteMessagesDialogComponent} from './delete-messages-dialog/delete-messages-dialog.component';
import {DialogConfig} from '../../shared/utils/dialog-config';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-import-view',
  templateUrl: './import-view.component.html',
  styleUrls: ['./import-view.component.css']
})
export class ImportViewComponent implements OnInit, OnDestroy {

  imports: MessagesImportList;
  deleting = false;
  refreshed = false;
  dataSubscription: Subscription;

  constructor(private route: ActivatedRoute,
              private messagesImportService: MessagesImportService,
              private dialog: MatDialog) { }

  ngOnInit() {
    this.dataSubscription = this.route.data.subscribe(data => {
      this.imports = data.imports;
      this.refreshed = false;
    });
  }
  ngOnDestroy() {
    this.dataSubscription.unsubscribe();
  }
  async delete(importId: string) {
    const dialogRef = this.dialog.open(DeleteMessagesDialogComponent, DialogConfig);

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
  async refresh() {
    this.imports = await this.messagesImportService.refresh();
    this.refreshed = true;
  }
  isBeingDeleted(): boolean {
    return !!this.imports.imports
      .find(x => x.isBeingDeleted);
  }
}
