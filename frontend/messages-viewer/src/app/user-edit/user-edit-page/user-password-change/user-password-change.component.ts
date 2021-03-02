import {Component, ElementRef, OnInit, ViewChild} from '@angular/core';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import {UserEditConfirmPassword} from './user-edit-confirm-password';
import {SessionStorageService} from '../../../services/session-storage.service';
import {ConfirmPasswordErrorMatcher} from '../../../shared/utils/confirm-password-error-state-matcher';
import {UserAccountService} from '../../../services/user-account.service';
import {ToastrService} from 'ngx-toastr';

@Component({
  selector: 'app-user-password-change',
  templateUrl: './user-password-change.component.html',
  styleUrls: ['./user-password-change.component.css']
})
export class UserPasswordChangeComponent implements OnInit {

  @ViewChild('passwordChangedMsg', {static: false})
  translatedMessage: ElementRef;

  passwordChangeForm: FormGroup;
  confirmPassword = new UserEditConfirmPassword();
  errorMatcher = new ConfirmPasswordErrorMatcher();


  constructor(private formBuilder: FormBuilder,
              private passwordChangeService: UserAccountService,
              private toastr: ToastrService) { }

  ngOnInit(): void {
    this.buildForm();
  }
  buildForm(): void {
    this.passwordChangeForm = this.formBuilder.group({
      oldPassword: ['', [
        Validators.required
      ]],
      newPassword: ['', [
        Validators.required
      ]],
      confirmNewPassword: ['', [Validators.required]],
    }, {
      validator: this.confirmPassword.matchPassword
    } );
  }
  changePassword(): void {
    this.passwordChangeService.changePassword(
      this.passwordChangeForm.value
    ).toPromise().then((response) => {
        this.toastr.success(this.translatedMessage.nativeElement.innerHTML);
    });
  }

}
