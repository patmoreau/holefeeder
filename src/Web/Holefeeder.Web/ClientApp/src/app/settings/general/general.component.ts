import { Component, OnInit } from '@angular/core';
import { ISettings } from '@app/shared/interfaces/settings.interface';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { SettingsService } from '@app/singletons/services/settings.service';
import {
  DateIntervalTypeNames
} from '@app/shared/enums/date-interval-type.enum';
import { NgbDateAdapter } from '@ng-bootstrap/ng-bootstrap';
import { Settings } from '@app/shared/models/settings.model';
import { NgbDateParserAdapter } from '@app/shared/ngb-date-parser.adapter';
import { faCalendarAlt } from '@fortawesome/free-solid-svg-icons';

@Component({
  selector: 'dfta-general',
  templateUrl: './general.component.html',
  styleUrls: ['./general.component.scss'],
  providers: [{ provide: NgbDateAdapter, useClass: NgbDateParserAdapter }]
})
export class GeneralComponent implements OnInit {
  isLoaded = false;
  settings: ISettings;
  isDirty = true;

  settingsForm: FormGroup;
  intervalTypesNames = DateIntervalTypeNames;

  faCalendarAlt = faCalendarAlt;

  constructor(
    private activatedRoute: ActivatedRoute,
    private settingsService: SettingsService,
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
    this.settingsService.settings.subscribe(settings => {
      this.settings = settings;
      this.settingsForm.patchValue(this.settings);
      this.isLoaded = true;
    });
  }

  async onSubmit() {
    const settings = Object.assign(new Settings(), this.settingsForm.value);

    await this.settingsService.update(settings);
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
