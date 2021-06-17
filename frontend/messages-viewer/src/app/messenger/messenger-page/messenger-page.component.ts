import { Component, OnInit } from '@angular/core';
import {ActivatedRoute} from '@angular/router';
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

  constructor(private route: ActivatedRoute,
              private importService: ImportService) { }

  ngOnInit() {
    this.importProgress = new ImportProgressData();
    this.imports = this.route.snapshot.data.imports;
  }
  async refresh() {
    this.imports = await this.importService.getAll();
  }
}
