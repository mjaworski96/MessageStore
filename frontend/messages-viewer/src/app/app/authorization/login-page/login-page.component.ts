import { Component, OnInit } from '@angular/core';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import {AuthorizationService} from '../services/authorization.service';

@Component({
  selector: 'app-login-page',
  templateUrl: './login-page.component.html',
  styleUrls: ['./login-page.component.css']
})
export class LoginPageComponent implements OnInit {

  loginForm: FormGroup;

  maxUsernameLength = 20;

  constructor(private formBuilder: FormBuilder,
              private authorizationService: AuthorizationService) { }

  ngOnInit(): void {
    this.buildForm();
  }
  buildForm(): void {
    this.loginForm = this.formBuilder.group({
      username: ['', [
        Validators.required,
        Validators.maxLength(this.maxUsernameLength)
      ]],
      password: ['', [
        Validators.required
      ]],
    });
  }
  login(): void {
    this.authorizationService.login(this.loginForm.value);
  }

}
