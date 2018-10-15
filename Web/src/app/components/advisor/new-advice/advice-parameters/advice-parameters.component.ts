import { Component, OnInit, Input, ViewChild } from '@angular/core';
import { InputType } from '../../../../model/inheritanceInputOptions';
import { InheritanceInputComponent } from '../../../util/inheritance-input/inheritance-input.component';

@Component({
  selector: 'advice-parameters',
  templateUrl: './advice-parameters.component.html', 
  styleUrls: ['./advice-parameters.component.css']
})
export class AdviceParametersComponent implements OnInit {
  @Input() currentPrice: number;
  @Input() adviceType: number;
  @ViewChild("TargetPrice") TargetPrice: InheritanceInputComponent;
  @ViewChild("StopLoss") StopLoss: InheritanceInputComponent;
  targetPriceValue?: number;
  stopLossValue?: number;
  stopLossOptions: any = { inputType: InputType.Number, textOptions: { placeHolder: "Stop loss (USD)", required: false, showHintSize: false } };
  targetOptions: any = { inputType: InputType.Number, textOptions: { placeHolder: "Target price (USD)", required: false, showHintSize: false } };

  constructor() { }
  
  ngOnInit() {
    let numberOptions;
    if (this.currentPrice >= 100) {
      numberOptions = { step: 1 };
    } else if (this.currentPrice >= 10) {
      numberOptions = { step: 0.1 };
    } else if (this.currentPrice >= 0.1) {
      numberOptions = { step: 0.01 };
    } else if (this.currentPrice >= 0.01) {
      numberOptions = { step: 0.001 };
    } else if (this.currentPrice >= 0.001) {
      numberOptions = { step: 0.0001 };
    } else if (this.currentPrice >= 0.0001) {
      numberOptions = { step: 0.00001 };
    } else if (this.currentPrice >= 0.00001) {
      numberOptions = { step: 0.000001 };
    } else {
      numberOptions = { step: 0.0000001 };
    }
    this.stopLossOptions["numberOptions"] = numberOptions;
    this.targetOptions["numberOptions"] = numberOptions;
    this.targetPriceValue = null;
    this.stopLossValue = null;
  }

  public isValidParameters(): boolean {
    let isValid = this.TargetPrice.isValid();
    isValid = this.StopLoss.isValid() && isValid;
    return isValid;
  }

  private onTargetPrice(newValue?: number) {
    this.targetPriceValue = newValue;
    if (this.targetPriceValue && ((this.adviceType == 1 && this.targetPriceValue <= this.currentPrice) ||
        (this.adviceType == 0 && this.targetPriceValue >= this.currentPrice))) {
      this.TargetPrice.setForcedError("Invalid target price for current value");
    } else {
      this.TargetPrice.setForcedError(null);
    }
  }

  private onStopLoss(newValue?: number) {
    this.stopLossValue = newValue;
    if (this.stopLossValue && ((this.adviceType == 1 && this.stopLossValue >= this.currentPrice) ||
        (this.adviceType == 0 && this.stopLossValue <= this.currentPrice))) {
      this.StopLoss.setForcedError("Invalid stop loss for current value");
    } else {
      this.StopLoss.setForcedError(null);
    }
  }
}