import {Component, Inject, OnInit} from '@angular/core';
import {AbstractControl, FormArray, FormControl, FormGroup, Validators} from '@angular/forms';
import {AliasWithId, AliasWithIdList} from '../../../model/alias';
import {AliasService} from '../../../services/alias.service';
import {ActivatedRoute, Router} from '@angular/router';
import {MAT_DIALOG_DATA, MatDialogRef} from '@angular/material';

@Component({
  selector: 'app-update-alias-name-dialog',
  templateUrl: './edit-alias-name-dialog.component.html',
  styleUrls: ['./edit-alias-name-dialog.component.css']
})
export class EditAliasNameDialogComponent implements OnInit {

  formGroup: FormGroup;
  alias: AliasWithId;
  aliasService: AliasService;
  constructor(@Inject(MAT_DIALOG_DATA) public data: any,
              public dialogRef: MatDialogRef<EditAliasNameDialogComponent>,) {
    this.alias = data.alias;
    this.aliasService = data.aliasService;
  }

  async ngOnInit() {
    this.buildForm();
  }
  buildForm() {
    this.formGroup = new FormGroup({
      name: new FormControl(this.alias.name, [Validators.required, Validators.maxLength(256)]),
    });
    this.formGroup.markAllAsTouched();
  }

  async save() {
    if (this.formGroup.valid) {
      await this.aliasService.editName(this.alias.id, this.formGroup.value).toPromise();
      this.dialogRef.close(this.formGroup.value.name);
    }
  }
}
