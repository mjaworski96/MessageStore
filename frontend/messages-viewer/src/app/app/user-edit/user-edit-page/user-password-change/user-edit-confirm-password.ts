import {AbstractControl} from '@angular/forms';
import {SharedConfirmPassword} from '../../../shared/utils/shared-confirm-password';

export class UserEditConfirmPassword extends SharedConfirmPassword {
  matchPassword(abstractControl: AbstractControl) {
    return super.matchPasswordHelper(abstractControl, 'newPassword',
      'confirmNewPassword');
  }
}
