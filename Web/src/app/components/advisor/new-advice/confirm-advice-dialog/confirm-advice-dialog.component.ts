import { Component, OnInit,Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA} from '@angular/material';
import { Util } from '../../../../util/Util';

@Component({
  selector: 'confirm-advice-dialog',
  templateUrl: './confirm-advice-dialog.component.html',
  styleUrls: ['./confirm-advice-dialog.component.css']
})
export class ConfirmAdviceDialogComponent implements OnInit {
  advancedSettings: string = null;
  targetPrice: number;
  stopLoss: number;

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
}
