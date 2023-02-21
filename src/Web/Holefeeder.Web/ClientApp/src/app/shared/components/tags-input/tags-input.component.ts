import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';
import { FormArray, FormBuilder } from '@angular/forms';

@Component({
  selector: 'app-tags-input',
  templateUrl: './tags-input.component.html',
  styleUrls: ['./tags-input.component.scss'],
  standalone: true,
  imports: [CommonModule],
})
export class TagsInputComponent {
  @Input() tagsArray: FormArray<any> = new FormArray<any>([]);
  @Input() isReadonly = false;

  constructor(private fb: FormBuilder) {}

  addTag(newTag: string) {
    if (this.isReadonly) {
      return;
    }
    if (
      newTag &&
      newTag !== '' &&
      !this.tagsArray.controls.find(ctl => ctl.value === newTag)
    ) {
      this.tagsArray.push(this.fb.control(newTag));
    }
  }

  removeTag(index: number) {
    if (this.isReadonly) {
      return;
    }
    this.tagsArray.removeAt(index);
  }
}
