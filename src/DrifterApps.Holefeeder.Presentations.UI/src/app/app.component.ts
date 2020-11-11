import {Component, OnInit} from '@angular/core';

@Component({
    selector: 'dfta-root',
    templateUrl: './app.component.html',
})
export class AppComponent implements OnInit {
    isIframe = false;
    loggedIn = false;

    constructor() {
    }

    ngOnInit() {
    }
}