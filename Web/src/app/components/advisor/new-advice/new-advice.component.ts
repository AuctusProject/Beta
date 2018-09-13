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
import { CoinSearchComponent } from '../../util/coin-search/coin-search.component';
import { MessageFullscreenModalComponent } from '../../util/message-fullscreen-modal/message-fullscreen-modal.component';
import { Util } from '../../../util/Util';

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

  @ViewChild("CoinSearch") CoinSearch: CoinSearchComponent;
  advise: AdviseRequest = new AdviseRequest();
  showSell: boolean = false;
  showButtons: boolean = false;
  asset: Asset;

  constructor(private assetService: AssetService, 
    private advisorService: AdvisorService, 
    private accountService : AccountService, 
    private notificationService: NotificationsService,
    private navigationService : NavigationService,
    public dialog: MatDialog) { }

  ngOnInit() {
    let loginData = this.accountService.getLoginData();
    if (!loginData) {
      this.setClose.emit();
      this.navigationService.goToLogin();
    } else if (!loginData.isAdvisor) {
      this.setClose.emit();
      this.navigationService.goToBecomeAdvisor();
    } 
    this.CoinSearch.onSelect.subscribe(newValue => 
      {
        this.asset = newValue;
        if (this.asset) {
          this.advise.assetId = this.asset.id;
        } else {
          this.advise.assetId = 0;
        }
        this.setButtons();
      });
    if (!!this.data && this.data.assetId) {
      this.CoinSearch.setForcedCoin(this.data.assetId);
    }
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
        data: { adviceType: this.advise.adviceType, assetName: this.asset.code + ' - ' + this.asset.name } }); 

    dialogRef.afterClosed().subscribe(result => {
      if(result) {
        this.advisorService.advise(this.advise).subscribe(ret => 
          {
            let recommendation = Util.GetRecommendationTypeDescription(this.advise.adviceType);
            let modalData = new FullscreenModalComponentInput();
            modalData.hiddenClose = true;
            modalData.component = MessageFullscreenModalComponent;
            modalData.componentInput = { message: "New " + recommendation + " recommendation was successfully created to " + this.asset.code + " - " + this.asset.name, reload: true };
            this.setNewModal.emit(modalData);
          });
      }
    });
  }

  getSearchOptions() {
    return { required: true, outlineField: true, darkStyle: false };
  }

  setButtons() {
    this.showButtons = !!this.asset && !!this.asset.id;
    this.showSell = !!this.asset && this.asset.shortSellingEnabled;
  }  
}
