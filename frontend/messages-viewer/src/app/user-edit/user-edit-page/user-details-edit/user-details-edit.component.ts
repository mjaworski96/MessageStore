import {Component, ElementRef, OnInit, ViewChild} from '@angular/core';
import {LoggedUser} from '../../../model/user';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import {UserAccountService} from '../../../services/user-account.service';
import {ActivatedRoute, Router} from '@angular/router';
import {HttpResponse} from '@angular/common/http';
import {ErrorHandlingService} from '../../../services/error-handling.service';
import {SessionStorageService} from '../../../services/session-storage.service';
import {ToastrService} from 'ngx-toastr';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-user-details-edit',
  templateUrl: './user-details-edit.component.html',
  styleUrls: ['./user-details-edit.component.css']
})
export class UserDetailsEditComponent implements OnInit {

  @ViewChild('accountUpdatedMsg', {static: false})
  translatedMessage: ElementRef;

  user: LoggedUser;
  userDetailsForm: FormGroup;
  maxUsernameLength = 20;
  dataSubscription: Subscription;

  constructor(private formBuilder: FormBuilder,
              private userAccountService: UserAccountService,
              private router: Router,
              private route: ActivatedRoute,
              private toastr: ToastrService) { }

  ngOnInit() {
    this.dataSubscription = this.route.data.subscribe(data => {
      this.user = data.user;
      this.buildForm();
    });
  }
  ngOnDestroy() {
    this.dataSubscription.unsubscribe();
  }
  buildForm(): void {
    this.userDetailsForm = this.formBuilder.group({
      username: [this.user.username, [
        Validators.required,
        Validators.maxLength(this.maxUsernameLength)
      ]],
      email: [this.user.email, Validators.required],
    });
  }
  update(): void {
    this.userAccountService.updateAccount(
      this.userDetailsForm.value
    ).toPromise().then((result: HttpResponse <LoggedUser>) => {
      this.toastr.success(this.translatedMessage.nativeElement.innerHTML);
      this.router.navigated = false;
      this.router.navigate(['/', 'account']);
    });
  }
}
