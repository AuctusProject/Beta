import { Component, OnInit, Input } from '@angular/core';

@Component({
  selector: 'percentage-display',
  templateUrl: './percentage-display.component.html',
  styleUrls: ['./percentage-display.component.css']
})
export class PercentageDisplayComponent implements OnInit {
  @Input() value: number;
  constructor() { }

  ngOnInit() {
  }

}
