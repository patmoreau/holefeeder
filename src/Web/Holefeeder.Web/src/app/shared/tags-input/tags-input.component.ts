import { Component, OnInit, Input, forwardRef } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR, FormArray, FormControl, FormBuilder } from '@angular/forms';
import { faPlus, faTimes } from '@fortawesome/free-solid-svg-icons';

@Component({
  selector: 'dfta-tags-input',
  templateUrl: './tags-input.component.html',
  styleUrls: ['./tags-input.component.scss']
})
export class TagsInputComponent implements OnInit {
  faPlus = faPlus;
  faTimes = faTimes;

  @Input() tagsArray: FormArray;

  constructor(private fb: FormBuilder) { }

  ngOnInit() { }

  addTag(newTag: string) {
    if (newTag && newTag !== '' && !this.tagsArray.controls.find(ctl => ctl.value === newTag)) {
      this.tagsArray.push(this.fb.control(newTag));
    }
  }

  removeTag(index: number) {
    this.tagsArray.removeAt(index);
  }
}
