import { Component, OnInit, Input, Output, EventEmitter, Inject, ViewChild } from '@angular/core';
import { MatDialog, MAT_DIALOG_DATA, MatDialogRef } from '@angular/material';
import { AssetResponse } from '../../../../model/asset/assetResponse';

@Component({
  selector: 'new-trade-window',
  templateUrl: './new-trade-window.component.html',
  styleUrls: ['./new-trade-window.component.css']
})
export class NewTradeWindowComponent implements OnInit {
  asset: AssetResponse;

  constructor(public dialogRef: MatDialogRef<NewTradeWindowComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any) { }

  ngOnInit() {
    this.asset = this.data.asset;
  }

  getTitle() : string {
      return "New " + this.asset.code + " order";
  }

  onOrderCreated() {
    this.dialogRef.close(true);
  }
}
