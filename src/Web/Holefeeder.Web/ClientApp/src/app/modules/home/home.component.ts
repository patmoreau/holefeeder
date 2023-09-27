import { Component, OnInit } from '@angular/core';
import { FooterComponent } from '@app/modules/home/footer/footer.component';
import { HeaderComponent } from '@app/modules/home/header/header.component';
import { ToastViewComponent } from '@app/shared/components';
import { LoadingBarModule } from '@ngx-loading-bar/core';
import { LoadingBarRouterModule } from '@ngx-loading-bar/router';
import { Store } from '@ngrx/store';
import { AppStore } from '@app/core/store';
import { AuthActions } from '@app/core/store/auth/auth.actions';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss'],
  standalone: true,
  imports: [
    LoadingBarRouterModule,
    LoadingBarModule,
    ToastViewComponent,
    HeaderComponent,
    FooterComponent,
  ],
})
export class HomeComponent implements OnInit {
  constructor() {}

  ngOnInit() {}
}
