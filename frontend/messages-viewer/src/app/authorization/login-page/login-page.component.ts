import { Component, OnInit } from '@angular/core';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import {AuthorizationService} from '../../services/authorization.service';
import {ActivatedRoute} from '@angular/router';

@Component({
  selector: 'app-login-page',
  templateUrl: './login-page.component.html',
  styleUrls: ['./login-page.component.css']
})
export class LoginPageComponent implements OnInit {

  loginForm: FormGroup;

  maxUsernameLength = 20;

  constructor(private formBuilder: FormBuilder,
              private authorizationService: AuthorizationService,
              private route: ActivatedRoute) { }

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
    const navigateToParam = this.route.snapshot.queryParams.navigateTo;
    const navigateTo = navigateToParam ? [navigateToParam] : ['/', 'aliases'];
    this.authorizationService.login(this.loginForm.value, navigateTo);
  }

}
