import { Component, OnInit } from '@angular/core';
import { AdviseRequest } from '../../../model/advisor/adviseRequest';
import { AdvisorService } from '../../../services/advisor.service';
import { NotificationsService } from '../../../../../node_modules/angular2-notifications';
import { Asset } from '../../../model/asset/asset';
import { AssetService } from '../../../services/asset.service';
import { FormControl } from '../../../../../node_modules/@angular/forms';
import { Observable } from '../../../../../node_modules/rxjs';
import {map, startWith} from 'rxjs/operators';
import { MatAutocompleteSelectedEvent } from '../../../../../node_modules/@angular/material';
import {MatDialog} from '@angular/material';
import { ConfirmAdviceDialogComponent } from './confirm-advice-dialog/confirm-advice-dialog.component';

@Component({
  selector: 'new-advice',
  templateUrl: './new-advice.component.html', 
  styleUrls: ['./new-advice.component.css']
})
export class NewAdviceComponent implements OnInit {
  options: Asset[];
  advise: AdviseRequest = new AdviseRequest();
  myControl = new FormControl();
  filteredOptions: Observable<Asset[]>;
  allSignalOptions = [
  {
    value: 0,
    text: "SELL"
  },
  {
    value: 1,
    text: "BUY"
  },
  {
    value: 2,
    text: "CLOSE"
  },
];

  currentAssetSignalOptions = [];
  constructor(private assetService: AssetService, private advisorService: AdvisorService, private notificationService: NotificationsService, public dialog: MatDialog) { }

  ngOnInit() {
    this.assetService.getAssets().subscribe(result => {
      this.options = result;
      this.filteredOptions = this.myControl.valueChanges
      .pipe(
        startWith<string | Asset>(''),
        map(value => typeof value === 'string' ? value : value.name),
        map(name => name ? this._filter(name) : this.options.slice())
      );
    });
  }

  displayFn(asset?: Asset): string | undefined {
    return asset ? asset.name : undefined;
  }

  private _filter(name: string): Asset[] {
    const filterValue = name.toLowerCase();

    return this.options.filter(option => option.name.toLowerCase().indexOf(filterValue) === 0);
  }

  onSubmit(){
    const dialogRef = this.dialog.open(ConfirmAdviceDialogComponent, {width: '250px',data: {adviceType:this.advise.adviceType,assetName: this.myControl.value.name}}); 

    dialogRef.afterClosed().subscribe(result => {
      if(result){
        this.advisorService.advise(this.advise).subscribe(result => this.notificationService.success("New advise was successfully created."));
      }
    });
    
  }

  optionSelected(selected: MatAutocompleteSelectedEvent){
    if(selected && selected.option && selected.option.value)
      this.advise.assetId = selected.option.value.id;

      this.fillOptions();
  }

  onAssetInputBlur() {
    if (this.myControl.value && !this.myControl.value.id)
      this.myControl.setValue("");
    
    this.fillOptions();
  }

  fillOptions(){
    this.currentAssetSignalOptions = this.allSignalOptions;
    if(this.myControl.value && !this.myControl.value.shortSellingEnabled)
    this.currentAssetSignalOptions = this.currentAssetSignalOptions.slice(1,3);
  }  
}