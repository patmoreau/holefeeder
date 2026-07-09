import { Directive, ElementRef, OnInit, inject } from '@angular/core';

@Directive({
  selector: '[appAutofocus]',
  standalone: true,
})
export class AutofocusDirective implements OnInit {
  private elRef = inject(ElementRef);


  ngOnInit(): void {
    this.elRef.nativeElement.focus();
  }
}
