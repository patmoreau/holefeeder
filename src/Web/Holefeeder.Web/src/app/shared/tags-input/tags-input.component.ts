import { Component, OnInit, Input } from '@angular/core';
import { FormArray, FormBuilder } from '@angular/forms';

@Component({
  selector: 'dfta-tags-input',
  templateUrl: './tags-input.component.html',
  styleUrls: ['./tags-input.component.scss']
})
export class TagsInputComponent implements OnInit {

  @Input() tagsArray: FormArray = new FormArray([]);
  @Input() isReadonly: boolean = false;

  constructor(private fb: FormBuilder) { }

  ngOnInit() {
  }

  addTag(newTag: string) {
    if (this.isReadonly) {
      return;
    }
    if (newTag && newTag !== '' && !this.tagsArray.controls.find(ctl => ctl.value === newTag)) {
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
