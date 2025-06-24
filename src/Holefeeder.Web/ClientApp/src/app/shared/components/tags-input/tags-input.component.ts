
import { Component, ElementRef, Input, ViewChild, inject } from '@angular/core';
import { FormArray, FormBuilder, FormControl } from '@angular/forms';
import { AppStore, TagsFeature } from '@app/core/store';
import { NgbTypeahead, NgbTypeaheadModule, NgbTypeaheadSelectItemEvent } from '@ng-bootstrap/ng-bootstrap';
import { Store } from '@ngrx/store';
import {
  Observable,
  OperatorFunction,
  Subject,
  debounceTime,
  distinctUntilChanged,
  filter,
  map,
  merge,
  switchMap,
  take,
} from 'rxjs';

@Component({
  selector: 'app-tags-input',
  templateUrl: './tags-input.component.html',
  styleUrls: ['./tags-input.component.scss'],
  standalone: true,
  imports: [NgbTypeaheadModule]
})
export class TagsInputComponent {
  @Input() tagsArray = new FormArray<FormControl<string | null>>([]);
  @Input() isReadonly = false;

  @ViewChild('instance', { static: true }) instance!: NgbTypeahead;
  @ViewChild('newTag') newTagInput!: ElementRef; // Access the input element

  focus$ = new Subject<string>();
  click$ = new Subject<string>();

  private store = inject(Store<AppStore>);

  constructor(private fb: FormBuilder) { }

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

  search: OperatorFunction<string, readonly string[]> = (text$: Observable<string>) => {
    const debouncedText$ = text$.pipe(debounceTime(200), distinctUntilChanged());
    const clicksWithClosedPopup$ = this.click$.pipe(filter(() => !this.instance.isPopupOpen()));
    const inputFocus$ = this.focus$;

    return merge(debouncedText$, inputFocus$, clicksWithClosedPopup$).pipe(
      switchMap((term) =>
        this.store.select(TagsFeature.contains(term)).pipe(take(10), map(t => t.map(tag => tag!.tag))),
      ),
    );
  };

  selectItemFn = (value: string, event: NgbTypeaheadSelectItemEvent) => {
    this.addTag(value);
    // Clear the input field after adding the tag
    if (this.newTagInput && this.newTagInput.nativeElement) {
      this.newTagInput.nativeElement.value = '';
    }
    // prevent selection so value does not remain in input
    event.preventDefault();
    return null;
  };
}
