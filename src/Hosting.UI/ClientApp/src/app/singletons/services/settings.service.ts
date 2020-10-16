import { Injectable } from '@angular/core';
import { ApiService } from '@app/shared/services/api.service';
import { BehaviorSubject, Observable } from 'rxjs';
import { ISettings, settingsToServer } from '../../shared/interfaces/settings.interface';
import { Settings } from '../../shared/models/settings.model';
import { DateIntervalType } from '../../shared/enums/date-interval-type.enum';
import { ObjectData } from '../../shared/models/object-data.model';
import { IObjectData } from '../../shared/interfaces/object-data.interface';
import { filter, map } from 'rxjs/operators';
import { startOfToday } from 'date-fns';

@Injectable({
  providedIn: 'root'
})
export class SettingsService {
  private basePath = 'api/v1/objects';
  private objectData: IObjectData;
  private settings$ = new BehaviorSubject<ISettings>(Object.assign(
    new Settings(),
    {
      effectiveDate: startOfToday(),
      intervalType: DateIntervalType.monthly,
      frequency: 1
    }
  ) as ISettings);

  constructor(private api: ApiService) {}

  loadUserSettings() {
    this.api
      .get(`${this.basePath}`)
      .pipe(
        filter(data => data.length !== 0),
        map(data => Object.assign(new ObjectData(), data[0]) as IObjectData)
      )
      .subscribe(objectData => {
        this.objectData = objectData;
        return this.settings$.next(Object.assign(
          new Settings(),
          JSON.parse(objectData.data, (key: string, value: string) => {
            if (key === 'effectiveDate') {
              return new Date(
                +value.substr(0, 4),
                +value.substr(5, 2) - 1,
                +value.substr(8, 2),
                0, 0, 0, 0);
            }
            return value;
          })
        ) as ISettings);
      });
  }

  get settings(): Observable<ISettings> {
    return this.settings$.asObservable();
  }

  async update(settings: ISettings) {
    const objectData = Object.assign(new ObjectData());
    if (this.objectData) {
      this.objectData = Object.assign(this.objectData, {
        data: JSON.stringify(settingsToServer(settings))
      });
      await this.api
        .put(`${this.basePath}/${this.objectData.id}`, this.objectData)
        .toPromise();
    } else {
      this.objectData = Object.assign(
        new ObjectData(),
        await this.api
          .post(
            `${this.basePath}`,
            Object.assign(new ObjectData(), {
              code: 'settings',
              data: JSON.stringify(settings)
            })
          )
          .toPromise()
      );
    }
    this.settings$.next(settings);
  }
}
