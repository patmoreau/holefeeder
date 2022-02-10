import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import {
  DateIntervalTypeNames
} from '@app/shared/enums/date-interval-type.enum';
import { NgbDateAdapter } from '@ng-bootstrap/ng-bootstrap';
import { NgbDateParserAdapter } from '@app/shared/ngb-date-parser.adapter';
import { Observable, tap } from 'rxjs';
import { SettingsService } from '@app/core/services/settings.service';
import { Settings, SettingsAdapter } from '@app/core/models/settings.model';

@Component({
  selector: 'app-general',
  templateUrl: './general.component.html',
  styleUrls: ['./general.component.scss'],
  providers: [{ provide: NgbDateAdapter, useClass: NgbDateParserAdapter }]
})
export class GeneralComponent implements OnInit {
  settings$: Observable<Settings> | undefined;

  settingsForm: FormGroup;
  intervalTypesNames = DateIntervalTypeNames;

  constructor(
    private settingsService: SettingsService,
    private settingsAdapter: SettingsAdapter,
    private formBuilder: FormBuilder,
    private router: Router
  ) {
    this.settingsForm = this.formBuilder.group({
      effectiveDate: ['', Validators.required],
      intervalType: ['', Validators.required],
      frequency: ['', [Validators.required, Validators.min(1)]]
    });
  }

  ngOnInit() {
    this.settings$ = this.settingsService.settings$
      .pipe(
        tap(settings => this.settingsForm.patchValue(settings))
      )
  }

  onSubmit() {
    this.settingsService.saveSettings(this.settingsAdapter.adapt(this.settingsForm.value))
      .subscribe(_ => this.router.navigate(['/']));
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
