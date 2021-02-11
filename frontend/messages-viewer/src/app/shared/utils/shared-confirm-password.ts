import {AbstractControl} from '@angular/forms';

export class SharedConfirmPassword {
  matchPasswordHelper(abstractControl: AbstractControl, firstPassword: string,
                      secondPassword: string): {matchPassword: boolean} {
    const password = abstractControl.get(firstPassword).value;
    const confirmPassword = abstractControl.get(secondPassword).value;
    if (password !== confirmPassword) {
      return {matchPassword: true};
    } else {
      return null;
    }
  }
}
