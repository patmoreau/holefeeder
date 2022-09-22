import { Directive, ElementRef, OnInit } from '@angular/core';

@Directive({
  selector: '[appAutofocus]',
  standalone: true,
})
export class AutofocusDirective implements OnInit {
  constructor(private elRef: ElementRef) {}

  ngOnInit(): void {
    this.elRef.nativeElement.focus();
  }
}
