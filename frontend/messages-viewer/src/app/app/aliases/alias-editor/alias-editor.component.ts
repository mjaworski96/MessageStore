import { Component, OnInit } from '@angular/core';
import {AliasService} from '../../../services/alias.service';
import {ActivatedRoute, ActivatedRouteSnapshot, Router, RouterStateSnapshot} from '@angular/router';
import {AliasWithId} from '../../../model/alias';
import {AbstractControl, FormArray, FormControl, FormGroup, Validators} from '@angular/forms';

@Component({
  selector: 'app-alias-editor',
  templateUrl: './alias-editor.component.html',
  styleUrls: ['./alias-editor.component.css']
})
export class AliasEditorComponent implements OnInit {
  formGroup: FormGroup;
  originalAlias: AliasWithId;
  constructor(private aliasService: AliasService,
              private router: Router,
              private route: ActivatedRoute) { }

  async ngOnInit() {
    const aliases = await this.aliasService
      .getAll(true)
      .toPromise();
    const id = +this.route.snapshot.paramMap.get('id');
    if (id) {
      this.originalAlias = await this.aliasService.get(id).toPromise();
      if (this.originalAlias.internal) {
        await this.router.navigate(['aliases', 'new']);
      }
    }
    this.buildForm(aliases.aliases);
  }
  buildForm(allAliases: AliasWithId[]) {
    const contactsForm = new FormArray([], [this.atLeastTwoElements]);

    for (let i = 0; i < allAliases.length; i++) {
      const element = allAliases[i];
      contactsForm.push(new FormGroup({
        id: new FormControl(element.id),
        name: new FormControl(element.name),
        application: new FormControl(element.application),
        selected: new FormControl(this.isSelected(element.id))
      }));
    }

    this.formGroup = new FormGroup({
      name: new FormControl(this.getName(), [Validators.required]),
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

    const alias = {
      name: this.formGroup.get('name').value,
      members: contacts
    };
    const edit = !this.originalAlias ?
      this.aliasService.create(alias) : this.aliasService.edit(this.originalAlias.id, alias);

    edit.toPromise()
      .then(() => this.router.navigate(['aliases']));
  }
  getName(): string {
    if (this.originalAlias) {
      return this.originalAlias.name;
    } else {
      return '';
    }
  }
  isSelected(id: number): boolean {
    if (this.originalAlias) {
      const filtered = this.originalAlias.members.filter(x => x.id === id);
      if (filtered.length > 0) {
        return true;
      }
    }
    return false;
  }
}
