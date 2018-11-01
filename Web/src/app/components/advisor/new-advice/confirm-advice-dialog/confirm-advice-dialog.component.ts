import { Component, OnInit,Inject, ViewChild } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA} from '@angular/material';
import { Util } from '../../../../util/Util';
import { TickerFieldComponent } from '../../../util/ticker-field/ticker-field.component';

@Component({
  selector: 'confirm-advice-dialog',
  templateUrl: './confirm-advice-dialog.component.html',
  styleUrls: ['./confirm-advice-dialog.component.css']
})
export class ConfirmAdviceDialogComponent implements OnInit {
  advancedSettings: string = null;
  targetPrice: number;
  stopLoss: number;
  @ViewChild("Ticker") Ticker: TickerFieldComponent;

  constructor(
    public dialogRef: MatDialogRef<ConfirmAdviceDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any) {}

  ngOnInit() {
    if (this.data.targetPrice) this.targetPrice = parseFloat(this.data.targetPrice);
    if (this.data.stopLoss) this.stopLoss = parseFloat(this.data.stopLoss);
  }

  getAdviceType(){
    return Util.GetRecommendationTypeDescription(this.data.adviceType);
  }

  onConfirm() {
    let currentValue = null;
    if (this.Ticker) {
      currentValue = this.Ticker.value;
    }
    this.dialogRef.close({ confirm: true, value: currentValue });
  }
}
