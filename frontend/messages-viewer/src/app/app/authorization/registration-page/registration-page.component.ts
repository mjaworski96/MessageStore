import { Component, OnInit } from '@angular/core';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import {RegistrationConfirmPassword} from './registration-confirm-password';
import {ConfirmPasswordErrorMatcher} from '../../shared/utils/confirm-password-error-state-matcher';
import {AuthorizationService} from '../services/authorization.service';

@Component({
  selector: 'app-registration-page',
  templateUrl: './registration-page.component.html',
  styleUrls: ['./registration-page.component.css']
})
export class RegistrationPageComponent implements OnInit {

  registerForm: FormGroup;
  confirmPassword = new RegistrationConfirmPassword();
  errorMatcher = new ConfirmPasswordErrorMatcher();

  maxUsernameLength = 20;

  constructor(private formBuilder: FormBuilder,
              private authorizationService: AuthorizationService) { }

  ngOnInit(): void {
    this.buildForm();
  }
  buildForm(): void {
    this.registerForm = this.formBuilder.group({
      username: ['', [
        Validators.required,
        Validators.maxLength(this.maxUsernameLength)
      ]],
      password: ['', [
        Validators.required
      ]],
      confirmPassword: ['', [Validators.required]],
      email: ['', [Validators.required]],
    }, {
      validator: this.confirmPassword.matchPassword
    } );
  }
  register(): void {
    this.authorizationService.register(this.registerForm.value);
  }

}
