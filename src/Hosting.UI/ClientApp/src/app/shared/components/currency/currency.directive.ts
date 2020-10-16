import { Directive, ElementRef, HostListener, OnInit } from '@angular/core';
import { CurrencyPipe } from '@angular/common';

@Directive({ selector: '[dftaCurrency]' })
export class CurrencyDirective implements OnInit {

  private currencyPipe = new CurrencyPipe('en-US');
  private el: HTMLInputElement;

  constructor(
    private elementRef: ElementRef,
  ) {
    this.el = this.elementRef.nativeElement;
  }

  ngOnInit() {
    this.el.value = this.currencyPipe.transform(this.el.value);
  }

  @HostListener('focus', ['$event.target.value'])
  onFocus(value) {
    this.el.value = '123'; // this.currencyPipe.parse(value);
  }

  @HostListener('blur', ['$event.target.value'])
  onBlur(value) {
    this.el.value = this.currencyPipe.transform(value);
  }
}
