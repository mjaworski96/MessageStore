import { Component, OnInit } from '@angular/core';
import {ActivatedRoute} from '@angular/router';
import { Subscription } from 'rxjs';
import {LoggedUser} from '../../model/user';

@Component({
  selector: 'app-user-edit-page',
  templateUrl: './user-edit-page.component.html',
  styleUrls: ['./user-edit-page.component.css']
})
export class UserEditPageComponent implements OnInit {
  user: LoggedUser;
  dataSubscription: Subscription;

  constructor(private route: ActivatedRoute) { }

  ngOnInit() {
    this.dataSubscription = this.route.data.subscribe(data => {
      this.user = data.user;
    });
  }
  ngOnDestroy() {
    this.dataSubscription.unsubscribe();
  }
}
