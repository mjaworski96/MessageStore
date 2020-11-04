import {ErrorStateMatcher} from '@angular/material';
import {FormControl, FormGroupDirective, NgForm} from '@angular/forms';

export class ConfirmPasswordErrorMatcher implements  ErrorStateMatcher {
  isErrorState(control: FormControl | null, form: FormGroupDirective | NgForm | null): boolean {
    return  control.touched && (control.parent.hasError('matchPassword') || control.invalid);
  }
}
