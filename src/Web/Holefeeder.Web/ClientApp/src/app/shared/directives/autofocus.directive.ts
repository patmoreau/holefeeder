import {Directive, ElementRef, Inject, Input, OnInit} from '@angular/core';
import {DOCUMENT} from '@angular/common';

@Directive({
  selector: '[dftaAutofocus]'
})
export class AutofocusDirective implements OnInit {
  private host: HTMLInputElement;
  private focused: Element | null;
  private autoFocus = true;

  constructor(private elRef: ElementRef, @Inject(DOCUMENT) private document: Document) {
    this.host = this.elRef.nativeElement;
    this.focused = this.document.activeElement;
  }

  @Input()
  set autofocus(value: boolean) {
    this.autoFocus = value;
  }

  ngOnInit(): void {
    if (this.autoFocus && this.host && this.host !== this.focused) {
      setTimeout(() => {
        this.host.focus();
        this.host.select();
      });
    }
  }
}
