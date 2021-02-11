import {Component, EventEmitter, OnInit, Output} from '@angular/core';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import {ImportService} from '../../../../services/import.service';

@Component({
  selector: 'app-messenger-import',
  templateUrl: './messenger-import.component.html',
  styleUrls: ['./messenger-import.component.css']
})
export class MessengerImportComponent implements OnInit {
  fileChunkSize = 4 * 1024 * 1024; // 4MB

  @Output()
  importAdded = new EventEmitter<void>();

  formGroup: FormGroup;
  blockButton = false;
  progressBarValue = 0;
  constructor(private formBuilder: FormBuilder,
              private importService: ImportService) { }

  ngOnInit() {
    this.formGroup = this.formBuilder.group({
      fbUsername: ['', Validators.required],
      file: ['', Validators.required],
      selectedFile: ['', Validators.required],
    });
  }
  onFileSelection(fileInput) {
    fileInput.click();
    this.formGroup.get('file').markAsTouched();
  }
  onFileChange(event) {
    if (event.target.files.length > 0) {
      const file = event.target.files[0];
      this.formGroup.patchValue({
        selectedFile: file
      });
    } else {
      this.formGroup.patchValue({
        file: '',
        selectedFile: ''
      });
      console.log('missing')
    }
  }
  async startImport() {
    this.blockButton = true;
    try {
      const importData = await this.importService.start({
        fbUsername: this.formGroup.value.fbUsername,
      });
      const file = this.formGroup.value.selectedFile as File;

      for (let i = 0; i < file.size; i += this.fileChunkSize) {
        const chunk = file.slice(i, this.fileChunkSize + i);

        const reader = new FileReader();
        reader.readAsDataURL(chunk);
        const chunkContent = await new Promise((resolve, reject) => {
          reader.onload = () => {
            resolve(reader.result);
          };
        });
        const skip = chunkContent.toString().indexOf(',') + 1;
        await this.importService.fileUpload(importData.id, {content: chunkContent.toString().substr(skip)});
        this.progressBarValue = 100 * (i + chunk.size) / file.size;
      }

      await this.importService.finish(importData.id);
      this.formGroup.reset();
      this.formGroup.get('fbUsername').markAsPristine();
      this.formGroup.get('file').markAsPristine();
      this.formGroup.get('selectedFile').markAsPristine();
    } finally {
       this.blockButton = false;
       this.progressBarValue = 0;
       this.importAdded.emit();
    }
  }
}
