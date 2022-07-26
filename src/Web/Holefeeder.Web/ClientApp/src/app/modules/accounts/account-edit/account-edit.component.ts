import {Component, OnInit} from '@angular/core';
import {FormGroup, FormGroupDirective} from '@angular/forms';
import {AccountTypeNames} from '@app/shared';

@Component({
  selector: 'app-account-edit',
  templateUrl: './account-edit.component.html',
  styleUrls: ['./account-edit.component.scss']
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
