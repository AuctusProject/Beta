import { Component, OnInit, Input, Output, EventEmitter, ViewChild, ElementRef } from '@angular/core';
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
import { ModalComponent } from '../../../model/modal/modalComponent';
import { FullscreenModalComponentInput } from '../../../model/modal/fullscreenModalComponentInput';
import { AccountService } from '../../../services/account.service';
import { NavigationService } from '../../../services/navigation.service';
import { ScrollStrategyOptions } from '@angular/cdk/overlay';

@Component({
  selector: 'new-advice',
  templateUrl: './new-advice.component.html', 
  styleUrls: ['./new-advice.component.css']
})
export class NewAdviceComponent implements ModalComponent, OnInit {
  modalTitle: string = "Set new recommendation";
  @Input() data: any;
  @Output() setClose = new EventEmitter<void>();
  @Output() setNewModal = new EventEmitter<FullscreenModalComponentInput>();

  options: Asset[];
  advise: AdviseRequest = new AdviseRequest();
  coinControl = new FormControl();
  filteredOptions: Observable<Asset[]>;
  showSell: boolean = false;
  showButtons: boolean = false;

  constructor(private assetService: AssetService, 
    private advisorService: AdvisorService, 
    private accountService : AccountService, 
    private notificationService: NotificationsService,
    private navigationService : NavigationService,
    public dialog: MatDialog) { }

  ngOnInit() {
    if (!this.accountService.isLoggedIn()) {
      this.setClose.emit();
      this.navigationService.goToLogin();
    } else {
      this.assetService.getAssets().subscribe(result => {
        this.options = result;
        this.filteredOptions = this.coinControl.valueChanges
        .pipe(
          startWith<string | Asset>(''),
          map(value => typeof value === 'string' ? value : value.name),
          map(name => name ? this._filter(name) : this.options.slice())
        );
      });
    }
  }

  displayFn(asset?: Asset): string | undefined {
    return !!asset ? asset.name : undefined;
  }

  private _filter(name: string): Asset[] {
    const filterValue = name.toLowerCase();

    return !!filterValue ? this.options.filter(option => option.name.toLowerCase().indexOf(filterValue) === 0
      || (filterValue.length > 1 && option.code.toLowerCase().indexOf(filterValue) === 0)) : [];
  }

  optionSelected(selected: MatAutocompleteSelectedEvent){
    if(!!selected && !!selected.option && selected.option.value) {
      this.advise.assetId = selected.option.value.id;
    }
    this.setButtons();
  }

  buy() {
    this.advise.adviceType = 1;
    this.openConfirmation();
  }

  sell() {
    this.advise.adviceType = 0;
    this.openConfirmation();
  }

  close() {
    this.advise.adviceType = 2;
    this.openConfirmation();
  }

  openConfirmation() {
    const dialogRef = this.dialog.open(ConfirmAdviceDialogComponent, 
      { width: '60%', height: '35%', hasBackdrop: true, disableClose: true, panelClass: 'fullscreen-modal', 
        data: { adviceType:this.advise.adviceType, assetName: this.coinControl.value.code + ' - ' + this.coinControl.value.name } }); 

    dialogRef.afterClosed().subscribe(result => {
      if(result) {
        this.advisorService.advise(this.advise).subscribe(ret => 
          {
            this.setClose.emit();
            this.notificationService.success("New recommendation was successfully created.");
          });
      }
    });
  }

  onAssetInputBlur() {
    if (!this.coinControl.value || this.coinControl.invalid) {
      this.coinControl.setValue("");
    }
    this.setButtons();
  }

  setButtons() {
    this.showButtons = !!this.coinControl.value && !!this.coinControl.value.id;
    this.showSell = !!this.coinControl.value && this.coinControl.value.shortSellingEnabled;
  }  
}
