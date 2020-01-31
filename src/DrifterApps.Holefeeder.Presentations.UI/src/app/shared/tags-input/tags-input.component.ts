import { Component, OnInit, Input, forwardRef } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';

@Component({
  selector: 'dfta-tags-input',
  templateUrl: './tags-input.component.html',
  styleUrls: ['./tags-input.component.scss'],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => TagsInputComponent),
      multi: true
    }
  ]
})
export class TagsInputComponent implements OnInit, ControlValueAccessor {
  @Input() label = 'tags';
  // tslint:disable-next-line:no-input-rename
  @Input('value') _tags: string[];
  @Input() placeholder = '';
  onChange: any = () => {};
  onTouched: any = () => {};

  get tags(): string[] {
    return this._tags;
  }

  set tags(val) {
    this._tags = val;
    this.onChange(val);
    this.onTouched();
  }

  constructor() {}

  ngOnInit() {}

  addTag(newTag: string) {
    if (!this.tags) {
      this.tags = [];
    }
    if (newTag && !this.tags.find(tag => tag === newTag)) {
      const tags = this.tags.slice(0);
      tags.push(newTag.toLocaleLowerCase());
      this.writeValue(tags);
    }
  }

  removeTag(oldTag: string) {
    if (oldTag && this.tags.find(tag => tag === oldTag)) {
      const tags = this.tags.slice(0);
      const ndx = tags.indexOf(oldTag);
      tags.splice(ndx, 1);
      console.log(tags);
      this.writeValue(tags);
    }
  }

  registerOnChange(fn: any) {
    this.onChange = fn;
  }

  registerOnTouched(fn: any) {
    this.onTouched = fn;
  }

  writeValue(value: string[]) {
    if (value) {
      this.tags = value;
    }
  }
}
