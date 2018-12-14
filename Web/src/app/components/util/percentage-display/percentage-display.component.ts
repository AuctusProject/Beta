import { Component, OnInit, Input, OnChanges } from '@angular/core';

@Component({
  selector: 'percentage-display',
  templateUrl: './percentage-display.component.html',
  styleUrls: ['./percentage-display.component.css']
})
export class PercentageDisplayComponent implements OnChanges {
  Math = Math;
  @Input() value: number;
  @Input() showArrow: boolean = true;
  constructor() { }

  ngOnChanges() {
    if (this.value == undefined || this.value == null) {
      this.value = 0;
    }
  }
}
