import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { SharedModule } from '@app/shared/shared.module';
import { AccountComponent } from './account/account.component';
import { GeneralComponent } from './general/general.component';
import { SettingsRoutingModule } from './settings-routing.module';
import { SettingsComponent } from './settings.component';

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    SharedModule,
    SettingsRoutingModule,
  ],
  declarations: [SettingsComponent, AccountComponent, GeneralComponent],
  providers: [],
})
export class SettingsModule {}
