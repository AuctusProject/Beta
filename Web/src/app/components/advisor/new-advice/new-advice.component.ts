import { Component, OnInit, Input, Output, EventEmitter, ViewChild } from '@angular/core';
import { AdviseRequest } from '../../../model/advisor/adviseRequest';
import { AdvisorService } from '../../../services/advisor.service';
import { Asset } from '../../../model/asset/asset';
import { MatDialog } from '@angular/material';
import { ConfirmAdviceDialogComponent } from './confirm-advice-dialog/confirm-advice-dialog.component';
import { ModalComponent } from '../../../model/modal/modalComponent';
import { FullscreenModalComponentInput } from '../../../model/modal/fullscreenModalComponentInput';
import { AccountService } from '../../../services/account.service';
import { NavigationService } from '../../../services/navigation.service';
import { CoinSearchComponent } from '../../util/coin-search/coin-search.component';
import { MessageFullscreenModalComponent } from '../../util/message-fullscreen-modal/message-fullscreen-modal.component';
import { Util } from '../../../util/Util';
import { AssetService } from '../../../services/asset.service';
import { AdviceParametersComponent } from './advice-parameters/advice-parameters.component';

@Component({
  selector: 'new-advice',
  templateUrl: './new-advice.component.html', 
  styleUrls: ['./new-advice.component.css']
})
export class NewAdviceComponent implements ModalComponent, OnInit {
  modalTitle: string;//Set new rating";
  @Input() data: any;
  @Output() setClose = new EventEmitter<void>();
  @Output() setNewModal = new EventEmitter<FullscreenModalComponentInput>();

  @ViewChild("CoinSearch") CoinSearch: CoinSearchComponent;
  @ViewChild("AdviceParameters") AdviceParameters: AdviceParametersComponent;
  advise: AdviseRequest = new AdviseRequest();
  showSell: boolean = false;
  showButtons: boolean = false;
  showClose: boolean = false;
  lastValue: number;
  asset: Asset;

  constructor(private advisorService: AdvisorService, 
    private accountService : AccountService, 
    private navigationService : NavigationService,
    private dialog: MatDialog,
    private assetService: AssetService) { }

  ngOnInit() {
    this.advise.adviceType = -1;
    let loginData = this.accountService.getLoginData();
    if (!loginData) {
      this.setClose.emit();
      this.navigationService.goToLogin();
    } else if (!loginData.isAdvisor) {
      this.setClose.emit();
      this.navigationService.goToCompleteRegistration();
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
    if (!!this.data) {
      if (this.data.assetId) {
        this.CoinSearch.setForcedCoin(this.data.assetId);
      }
      if (this.data.adviceType || this.data.adviceType === 0) {
        this.setAdviceType(this.data.adviceType);
      }
    }
  }

  buy() {
    this.setAdviceType(1);
  }

  sell() {
    this.setAdviceType(0);
  }

  close() {
    this.setAdviceType(2);
  }

  setAdviceType(type: number) {
    let initiate = this.advise.adviceType == -1;
    this.advise.adviceType = type;
    if (!initiate) {
      this.AdviceParameters.ngOnInit();
    }
  }

  openConfirmation() {
    this.advise.stopLoss = this.AdviceParameters ? this.AdviceParameters.stopLossValue : null;
    this.advise.targetPrice = this.AdviceParameters ? this.AdviceParameters.targetPriceValue : null;
    const dialogRef = this.dialog.open(ConfirmAdviceDialogComponent, 
      { width: '370px', height: '35%', hasBackdrop: true, disableClose: true, panelClass: 'fullscreen-modal', 
        data: 
        { 
          adviceType: this.advise.adviceType, 
          assetName: this.asset.code + ' - ' +  this.asset.name, 
          lastValue: this.lastValue,
          targetPrice: this.advise.targetPrice,
          stopLoss: this.advise.stopLoss
        }}); 

    dialogRef.afterClosed().subscribe(result => {
      if(result) {
        this.advisorService.advise(this.advise).subscribe(ret => 
          {
            let recommendation = Util.GetRecommendationTypeDescription(this.advise.adviceType);
            let modalData = new FullscreenModalComponentInput();
            modalData.hiddenClose = true;
            modalData.component = MessageFullscreenModalComponent;
            modalData.componentInput = { message: "New " + recommendation + " rating was successfully created to " + this.asset.code + " - " + this.asset.name, reload: true };
            this.setNewModal.emit(modalData);
          });
      }
    });
  }

  getSearchOptions() {
    return { required: true, outlineField: true, darkStyle: false };
  }

  setButtons() {
    this.showClose = false;
    this.showSell = false;
    this.lastValue = null;
    this.showButtons = false;
    if(!!this.asset && !!this.asset.id){
      this.assetService.getAssetRecommendationInfo(this.asset.id).subscribe(result => 
        {
          this.showClose = result.closeRecommendationEnabled;
          this.showSell = !!this.asset && this.asset.shortSellingEnabled;
          this.lastValue = result.lastValue;
          this.showButtons = true;
        })
    }
  }  

  getConfirmText() {
    let text = "CONFIRM ";
    if (this.advise.adviceType == 0) {
      return text + "SELL";
    }if (this.advise.adviceType == 1) {
      return text + "BUY";
    } else {
      return text + "CLOSE";
    }
  }

  onConfirmClick() {
    if (!this.AdviceParameters || this.AdviceParameters.isValidParameters()) {
      this.openConfirmation();
    }
  }
}
