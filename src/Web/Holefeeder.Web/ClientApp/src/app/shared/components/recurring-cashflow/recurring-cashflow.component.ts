import { Component, OnInit } from '@angular/core';
import { ControlContainer, FormGroup } from '@angular/forms';
import { DateIntervalTypeNames } from '@app/shared/models';

@Component({
  selector:
    '[formGroup] app-recurring-cashflow,[formGroupName] app-recurring-cashflow',
  templateUrl: './recurring-cashflow.component.html',
  styleUrls: ['./recurring-cashflow.component.scss'],
})
export class RecurringCashflowComponent implements OnInit {
  form!: FormGroup;

  intervalTypesNames = DateIntervalTypeNames;

  constructor(private controlContainer: ControlContainer) {}

  ngOnInit(): void {
    this.form = <FormGroup>this.controlContainer.control;
  }

  compareFn(optionOne: any, optionTwo: any): boolean {
    if (optionOne && optionTwo) {
      const idOne = typeof optionOne === 'string' ? optionOne : optionOne.id;
      const idTwo = typeof optionTwo === 'string' ? optionTwo : optionTwo.id;
      return idOne === idTwo;
    }
    return false;
  }
}
