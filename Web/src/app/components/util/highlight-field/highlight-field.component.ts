import { Component, Input, OnChanges, SimpleChanges } from '@angular/core';
import { ValueDisplayPipe } from 'src/app/util/value-display.pipe';

@Component({
  selector: 'highlight-field',
  templateUrl: './highlight-field.component.html',
  styleUrls: ['./highlight-field.component.css']
})
export class HighlightFieldComponent implements OnChanges {
  @Input() value: number;
  @Input() decimalsQty?: number = null;
  @Input() suffix: string = '';
  @Input() prefix: string = '';
  bear: boolean = false;
  bull: boolean = false;

  ngOnChanges(changes: SimpleChanges) {
    if (changes && !changes.value.isFirstChange()) {
      if (changes.value.previousValue > changes.value.currentValue) {
        this.setBearColor();
      } else if (changes.value.previousValue < changes.value.currentValue) {
        this.setBullColor();
      }
    }
  }

  getValue(): string {
    if (this.value == undefined || this.value == null) {
      return "";
    } else if (this.decimalsQty || this.decimalsQty == 0) {
      return this.prefix + this.value.toLocaleString(undefined, { minimumFractionDigits: this.decimalsQty, maximumFractionDigits: this.decimalsQty }) + this.suffix;
    } else {
      return new ValueDisplayPipe().transform(this.value, this.prefix) + this.suffix;
    }
  }

  setBearColor() {
    this.bear = true;
    setTimeout(() => { this.bear = false; }, 500);
  }

  setBullColor() {
    this.bull = true;
    setTimeout(() => { this.bull = false; }, 500);
  }
}
