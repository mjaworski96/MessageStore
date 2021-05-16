import { Component, OnInit } from '@angular/core';
import {ApkService} from '../services/apk.service';

@Component({
  selector: 'app-main-page',
  templateUrl: './main-page.component.html',
  styleUrls: ['./main-page.component.css']
})
export class MainPageComponent implements OnInit {

  constructor(private apkService: ApkService) { }

  ngOnInit() {
  }
  async downloadApk() {
    await this.apkService.downloadApk();
  }
}
