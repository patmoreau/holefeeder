import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';

import {SettingsComponent} from './settings.component';
import {AccountComponent} from './account/account.component';
import {GeneralComponent} from './general/general.component';
import {SettingsRoutingModule} from './settings-routing.module';
import {SharedModule} from "@app/shared/shared.module";

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    SharedModule,
    SettingsRoutingModule
  ],
  declarations: [SettingsComponent, AccountComponent, GeneralComponent],
  providers: []
})
export class SettingsModule {
}
