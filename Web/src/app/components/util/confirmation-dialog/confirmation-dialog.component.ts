import { Component, OnInit, Input, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material';

@Component({
  selector: 'confirmation-dialog',
  templateUrl: './confirmation-dialog.component.html',
  styleUrls: ['./confirmation-dialog.component.css']
})
export class ConfirmationDialogComponent implements OnInit {
  @Input() title:string;
  @Input() message:string;
  @Input() closeLabel:string;
  @Input() confirmLabel:string;
  constructor(public dialogRef: MatDialogRef<ConfirmationDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any) { }

  ngOnInit() {
    this.title = this.data.title;
    this.message = this.data.message;
    this.closeLabel = this.data.closeLabel;
    this.confirmLabel = this.data.confirmLabel;
  }

  onConfirm(){
    this.dialogRef.close(true);
  }
}
