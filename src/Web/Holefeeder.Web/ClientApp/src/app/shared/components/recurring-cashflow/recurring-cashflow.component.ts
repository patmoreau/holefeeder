import {Component, OnInit} from '@angular/core';
import {ControlContainer, FormGroup} from "@angular/forms";
import {DateIntervalTypeNames, NgbDateParserAdapter} from "@app/shared";
import {NgbDateAdapter} from "@ng-bootstrap/ng-bootstrap";

@Component({
  selector: '[formGroup] app-recurring-cashflow,[formGroupName] app-recurring-cashflow',
  templateUrl: './recurring-cashflow.component.html',
  styleUrls: ['./recurring-cashflow.component.scss'],
  providers: [{provide: NgbDateAdapter, useClass: NgbDateParserAdapter}]
})
export class RecurringCashflowComponent implements OnInit {

  form!: FormGroup;

  intervalTypesNames = DateIntervalTypeNames;

  constructor(
    private controlContainer: ControlContainer
  ) {
  }

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
