import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import {
  DateValidator,
  DateValidatorDirective,
} from '@app/shared/directives/date-validator.directive';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';

const COMPONENTS = [DateValidatorDirective];

@NgModule({
  declarations: [COMPONENTS],
  imports: [CommonModule, FormsModule, ReactiveFormsModule, NgbModule],
  exports: [COMPONENTS],
  providers: [DateValidator],
})
export class SharedModule {}
