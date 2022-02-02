import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'dfta-loader',
  templateUrl: './loader.component.html',
  styleUrls: ['./loader.component.scss']
})
export class LoaderComponent implements OnInit {
  @Input() small: boolean = false;

  constructor() {}

  ngOnInit() {}
}
