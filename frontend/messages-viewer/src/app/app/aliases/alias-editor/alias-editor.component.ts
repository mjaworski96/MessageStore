import { Component, OnInit } from '@angular/core';
import {AliasService} from '../../../services/alias.service';
import {Router} from '@angular/router';
import {AliasWithId} from '../../../model/alias';
import {AbstractControl, FormArray, FormControl, FormGroup, Validators} from '@angular/forms';

@Component({
  selector: 'app-alias-editor',
  templateUrl: './alias-editor.component.html',
  styleUrls: ['./alias-editor.component.css']
})
export class AliasEditorComponent implements OnInit {
  formGroup: FormGroup;
  constructor(private aliasService: AliasService,
              private router: Router) { }

  ngOnInit() {
    this.aliasService
      .getAll(true)
      .toPromise()
      .then((result) => {
        this.buildForm(result.aliases);
      });
  }
  buildForm(allAliases: AliasWithId[]) {
    const contactsForm = new FormArray([], [this.atLeastTwoElements]);

    for (let i = 0; i < allAliases.length; i++) {
      const element = allAliases[i];
      contactsForm.push(new FormGroup({
        id: new FormControl(element.id),
        name: new FormControl(element.name),
        application: new FormControl(element.application),
        selected: new FormControl(false)
      }));
    }

    this.formGroup = new FormGroup({
      name: new FormControl('', [Validators.required]),
      contacts: contactsForm
    });
  }
  getContactsForm(): AbstractControl[] {
    const contacts = this.formGroup.get('contacts') as FormArray;
    return contacts.controls;
  }
  atLeastTwoElements(control: AbstractControl) {
    const selected = (control as FormArray).controls
      .filter(x => x.get('selected').value === true)
      .length;
    if (selected < 2) {
      return {
        atLeastTwoElements: true,
      };
    } else {
      return null;
    }
  }
  send() {
    const contacts =
    (this.formGroup.get('contacts') as FormArray).controls
      .filter(x => x.get('selected').value === true)
      .map(x => {
        return {id: x.get('id').value};
      });
    this.aliasService.create(
      {
        name: this.formGroup.get('name').value,
        members: contacts
      }
    ).toPromise()
      .then(() => this.router.navigate(['aliases']));
  }
}
