import { Component, OnInit } from '@angular/core';
import {SessionStorageService} from '../../../services/session-storage.service';
import {Router} from '@angular/router';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.css']
})
export class HeaderComponent implements OnInit {

  constructor(private sessionStorageService: SessionStorageService,
              private router: Router) { }

  ngOnInit() {
  }
  isLoggedIn() {
    return this.sessionStorageService.isUserLoggedIn();
  }
  logout() {
    this.sessionStorageService.logout();
    this.router.navigate(['/']);
  }

}
