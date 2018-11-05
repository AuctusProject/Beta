import { Component, OnInit, Input, OnChanges } from '@angular/core';

@Component({
  selector: 'percentage-display',
  templateUrl: './percentage-display.component.html',
  styleUrls: ['./percentage-display.component.css']
})
export class PercentageDisplayComponent implements OnChanges {
  @Input() value: number;
  constructor() { }

  ngOnChanges() {
    this.value = Math.round(this.value * 10000) / 100;
  }

}
