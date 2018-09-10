import { Component, OnInit,Inject } from '@angular/core';

import {MatDialog, MatDialogRef, MAT_DIALOG_DATA} from '@angular/material';
import { AdviseRequest } from '../../../../model/advisor/adviseRequest';
import { Util } from '../../../../util/Util';

@Component({
  selector: 'confirm-advice-dialog',
  templateUrl: './confirm-advice-dialog.component.html',
  styleUrls: ['./confirm-advice-dialog.component.css']
})
export class ConfirmAdviceDialogComponent implements OnInit {

  constructor(
    public dialogRef: MatDialogRef<ConfirmAdviceDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any) {}

  ngOnInit() {
  }

  getAdviceType(){
    return Util.GetRecommendationTypeDescription(this.data.adviceType);
  }
}
