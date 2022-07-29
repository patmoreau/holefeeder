import {Component, OnInit} from '@angular/core';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import {Router} from '@angular/router';
import {Observable, tap} from 'rxjs';
import {SettingsService} from '@app/core/services/settings.service';
import {Settings, SettingsAdapter} from '@app/core/models/settings.model';
import {DateIntervalTypeNames} from "@app/shared";

@Component({
  selector: 'app-general',
  templateUrl: './general.component.html',
  styleUrls: ['./general.component.scss']
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
