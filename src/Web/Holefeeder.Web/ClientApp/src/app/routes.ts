import { Routes } from '@angular/router';

export const ROUTES: Routes = [
  {
    path: '',
    loadChildren: () =>
      import('./modules/home/routes').then(m => m.HOME_ROUTES),
  },
];
