import { Directive, Input, ElementRef, Inject, OnInit } from '@angular/core';
import { DOCUMENT } from '@angular/common';

@Directive({
  selector: '[dftaAutofocus]'
})
export class AutofocusDirective implements OnInit {
  private host: HTMLInputElement;
  private focused: Element;
  private autoFocus = true;

  @Input()
  set autofocus(value: boolean) {
    this.autoFocus = value;
  }

  constructor(private elRef: ElementRef, @Inject(DOCUMENT) private document: HTMLDocument) {
    this.host = elRef.nativeElement;
    this.focused = document.activeElement;
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
