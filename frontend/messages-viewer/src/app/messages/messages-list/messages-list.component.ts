import {AfterViewChecked, Component, ElementRef, HostListener, OnInit, ViewChild} from '@angular/core';
import {MessageService} from '../../services/message.service';
import {ActivatedRoute} from '@angular/router';
import {MessageWithId} from '../../model/message';
import {finalize} from 'rxjs/operators';

@Component({
  selector: 'app-messages-list',
  templateUrl: './messages-list.component.html',
  styleUrls: ['./messages-list.component.css']
})
export class MessagesListComponent implements OnInit, AfterViewChecked {
  static pageSize = 20;

  @ViewChild('scroll', {static: false})
  scrollContainer: ElementRef;
  mustScrollDown = false;
  whenScrollUp = 1;
  whenScrollDown = 1.5;

  messages: MessageWithId[];
  aliasId: number;

  minPage: number;
  maxPage: number;
  end = false;
  loading = false;

  constructor(private route: ActivatedRoute,
              private messageService: MessageService) {
  }

  ngOnInit() {
    this.messages = [];
    this.aliasId = +this.route.snapshot.queryParamMap.get('aliasId');
    const page = +this.route.snapshot.queryParamMap.get('page');
    this.minPage = page;
    this.maxPage = page;
    this.loadData(page, true);
  }

  @HostListener('window:scroll', ['$event'])
  customScroll(event) {
    if (window.pageYOffset < this.whenScrollUp * window.outerHeight) {
      this.onScrollUp();
    }
    if (this.scrollContainer.nativeElement.scrollHeight -  this.whenScrollDown * window.outerHeight < window.pageYOffset) {
      this.onScrollDown();
    }
  }

  ngAfterViewChecked(): void {
    if (this.mustScrollDown) {
      this.mustScrollDown = false;
      window.scrollTo(0, this.scrollContainer.nativeElement.scrollHeight);
    }
  }

  loadData(pageNumber: number, scrollDownAfterLoad: boolean) {
    if (!this.end && pageNumber > 0 && !this.loading) {
      this.loading = true;
      this.messageService
        .get(this.aliasId, pageNumber, MessagesListComponent.pageSize)
        .pipe(finalize(() => {
          this.loading = false;
        }))
        .toPromise()
        .then(result => {
          if (pageNumber < this.minPage) {
            this.minPage = pageNumber;
            this.messages = this.messages.concat(result.messages.reverse());
          } else if (pageNumber > this.maxPage) {
            this.maxPage = pageNumber;
            this.messages = result.messages.reverse().concat(this.messages);
          } else {
            this.messages = result.messages.reverse();
          }
          if (result.messages.length === 0) {
            this.end = true;
          }
          if (scrollDownAfterLoad) {
            this.mustScrollDown = true;
          }
        });
    }
  }

  onScrollUp(): void {
    this.loadData(this.maxPage + 1, false);
  }

  onScrollDown(): void {
    this.loadData(this.minPage - 1, false);
  }

  writtenByUser(message: MessageWithId): boolean {
    return message.writerType === 'app_user';
  }

  writtenByContact(message: MessageWithId): boolean {
    return message.writerType === 'contact';
  }
}

