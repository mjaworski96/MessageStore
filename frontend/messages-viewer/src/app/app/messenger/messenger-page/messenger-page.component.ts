import { Component, OnInit } from '@angular/core';
import {ActivatedRoute} from '@angular/router';
import {ImportsList} from '../../../model/import';
import {ImportService} from '../../../services/import.service';

@Component({
  selector: 'app-messenger-page',
  templateUrl: './messenger-page.component.html',
  styleUrls: ['./messenger-page.component.css']
})
export class MessengerPageComponent implements OnInit {

  imports: ImportsList;
  constructor(private route: ActivatedRoute,
              private importService: ImportService) { }

  ngOnInit() {
    this.imports = this.route.snapshot.data.imports;
  }
  async refresh() {
    this.imports = await this.importService.getAll();
  }
}
