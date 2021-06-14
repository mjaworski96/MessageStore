import {AfterViewInit, Component, ElementRef, Inject, LOCALE_ID, OnInit, ViewChild} from '@angular/core';
import {Title} from '@angular/platform-browser';
import { DOCUMENT } from '@angular/common';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements AfterViewInit {
  @ViewChild('title', {static: false})
  translatedTitle: ElementRef;

  constructor(private titleService: Title,
    @Inject(DOCUMENT) private document: Document,
    @Inject(LOCALE_ID) private locale: string) {}

  ngAfterViewInit(): void {
    this.titleService.setTitle(this.translatedTitle.nativeElement.innerHTML);
    this.document.documentElement.lang = this.locale;
  }

}
