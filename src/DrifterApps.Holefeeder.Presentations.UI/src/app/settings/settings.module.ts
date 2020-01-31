import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { SettingsComponent } from './settings.component';
import { AccountComponent } from './account/account.component';
import { GeneralComponent } from './general/general.component';
import { SettingsRoutingModule } from './settings-routing.module';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    FontAwesomeModule,
    NgbModule,
    SettingsRoutingModule
  ],
  declarations: [SettingsComponent, AccountComponent, GeneralComponent],
  providers: []
})
export class SettingsModule {}
