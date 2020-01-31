import { NgModule } from '@angular/core';
import { UpcomingService } from './services/upcoming.service';
import { DateService } from './services/date.service';
import { SettingsService } from './services/settings.service';
import { SharedModule } from '@app/shared/shared.module';

@NgModule({
  imports: [SharedModule],
  providers: [
    DateService,
    SettingsService,
    UpcomingService
  ]
})
export class SingletonsModule {}
