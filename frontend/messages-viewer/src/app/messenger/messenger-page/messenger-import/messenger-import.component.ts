import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import {ImportService} from '../../../services/import.service';
import {ImportProgressData} from '../messenger-page.component';

@Component({
  selector: 'app-messenger-import',
  templateUrl: './messenger-import.component.html',
  styleUrls: ['./messenger-import.component.css']
})
export class MessengerImportComponent implements OnInit {
  fileChunkSize = 4 * 1024 * 1024; // 4MB

  @Output()
  updateImports = new EventEmitter<void>();

  formGroup: FormGroup;
  @Input()
  importProgress: ImportProgressData;

  progressBarValue = 0;
  constructor(private formBuilder: FormBuilder,
              private importService: ImportService) { }

  ngOnInit() {
    this.formGroup = this.formBuilder.group({
      facebookName: ['', Validators.required],
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
    }
  }
  async startImport() {
    this.importProgress.importInProgress = true;
    try {
      const importData = await this.importService.start({
        facebookName: this.formGroup.value.facebookName,
      });
      this.updateImports.emit();
      const file = this.formGroup.value.selectedFile as File;

      for (let i = 0; i < file.size; i += this.fileChunkSize) {
        if (this.importProgress.importCanceled) {
          break;
        }

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

      if (this.importProgress.importCanceled) {
        await this.importService.cancel(importData.id);
      } else {
        await this.importService.finish(importData.id);
        this.importProgress.importCanceled = false;
      }

      this.formGroup.reset();
      this.formGroup.get('facebookName').markAsPristine();
      this.formGroup.get('file').markAsPristine();
      this.formGroup.get('selectedFile').markAsPristine();
    } finally {
       this.importProgress.importInProgress = false;
       this.progressBarValue = 0;
       this.updateImports.emit();
    }
  }
}
