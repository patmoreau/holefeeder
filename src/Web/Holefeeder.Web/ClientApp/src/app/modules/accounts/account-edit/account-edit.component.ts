import {Component, OnInit} from '@angular/core';
import {FormGroup, FormGroupDirective} from '@angular/forms';
import {AccountTypeNames, NgbDateParserAdapter} from '@app/shared';
import {NgbDateAdapter} from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-account-edit',
  templateUrl: './account-edit.component.html',
  styleUrls: ['./account-edit.component.scss'],
  providers: [{provide: NgbDateAdapter, useClass: NgbDateParserAdapter}]
})
export class AccountEditComponent implements OnInit {

  form!: FormGroup;

  accountTypesNames = AccountTypeNames;

  constructor(
    private rootFormGroup: FormGroupDirective,
  ) {
  }

  ngOnInit(): void {
    this.form = this.rootFormGroup.control;
  }
}
