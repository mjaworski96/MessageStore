import { Component, OnInit } from '@angular/core';
import {ActivatedRoute} from '@angular/router';
import { Subscription } from 'rxjs';
import {ImportsList} from '../../model/import';
import {ImportService} from '../../services/import.service';

export class ImportProgressData {
  importInProgress = false;
  importCanceled = false;
}

@Component({
  selector: 'app-messenger-page',
  templateUrl: './messenger-page.component.html',
  styleUrls: ['./messenger-page.component.css']
})
export class MessengerPageComponent implements OnInit {

  imports: ImportsList;
  importProgress: ImportProgressData;
  dataSubscription: Subscription;
  
  constructor(private route: ActivatedRoute,
              private importService: ImportService) { }

  ngOnInit() {
    this.dataSubscription = this.route.data.subscribe(data => {
      this.importProgress = new ImportProgressData();
      this.imports = data.imports;
    });
  }
  ngOnDestroy() {
    this.dataSubscription.unsubscribe();
  }
  async refresh() {
    this.imports = await this.importService.getAll();
  }
}
