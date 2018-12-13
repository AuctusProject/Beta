import { Injectable, PLATFORM_ID, Inject, EventEmitter } from '@angular/core';
import { FullscreenModalComponent } from '../components/util/fullscreen-modal/fullscreen-modal.component';
import { FullscreenModalComponentInput } from '../model/modal/fullscreenModalComponentInput';
import { MatDialog, MatDialogRef } from '@angular/material';
import { ConfirmEmailComponent } from '../components/account/confirm-email/confirm-email.component';
import { EntryOptionComponent } from '../components/account/entry-option/entry-option.component';
import { ForgotPasswordResetComponent } from '../components/account/forgot-password-reset/forgot-password-reset.component';
import { BecomeAdvisorComponent } from '../components/advisor/become-advisor/become-advisor.component';
import { ChangePasswordComponent } from '../components/account/change-password/change-password.component';
import { ReferralDetailsComponent } from '../components/account/referral-details/referral-details.component';
import { AdvisorEditComponent } from '../components/advisor/advisor-edit/advisor-edit.component';
import { isPlatformBrowser } from '@angular/common';
import { ConfirmationDialogComponent } from '../components/util/confirmation-dialog/confirmation-dialog.component';
import { EditOrderComponent } from '../components/trade/edit-order/edit-order.component';
import { OrderResponse } from '../model/trade/orderResponse';
import { EditTradeValueComponent } from '../components/trade/edit-trade-value/edit-trade-value.component';
import { AssetResponse } from '../model/asset/assetResponse';
import { NewTradeWindowComponent } from '../components/trade/new-trade/new-trade-window/new-trade-window.component';

@Injectable()
export class ModalService {
  constructor(
    @Inject(PLATFORM_ID) private platformId: Object,
    private dialog: MatDialog) {
  }

  private setModal(component: any, componentInputData?: any, hiddenClose: boolean = false, forcedTitle: string = undefined): MatDialogRef<FullscreenModalComponent, any> {
    if(isPlatformBrowser(this.platformId)){
      let modalData = new FullscreenModalComponentInput();
      modalData.component = component;
      modalData.componentInput = componentInputData;
      modalData.hiddenClose = hiddenClose;
      modalData.forcedTitle = forcedTitle;
      return this.dialog.open(FullscreenModalComponent, { data: modalData }); 
    }
    return null;
  }

  public setConfirmEmail(): MatDialogRef<FullscreenModalComponent, any> {
    return this.setModal(ConfirmEmailComponent);
  }

  public setRegister(): MatDialogRef<FullscreenModalComponent, any> {
    return this.setModal(EntryOptionComponent);
  }

  public setResetPassword(): MatDialogRef<FullscreenModalComponent, any> {
    return this.setModal(ForgotPasswordResetComponent);
  }

  public setBecomeAdvisor(): MatDialogRef<FullscreenModalComponent, any> {
    return this.setModal(EntryOptionComponent, { becomeExpert: true });
  }

  public setCompleteRegistration(): MatDialogRef<FullscreenModalComponent, any> {
    return this.setModal(EntryOptionComponent, { completeregistration: true });
  }

  public setBecomeAdvisorForm(): MatDialogRef<FullscreenModalComponent, any> {
    return this.setModal(BecomeAdvisorComponent);
  }

  public setLogin(): MatDialogRef<FullscreenModalComponent, any> {
    return this.setModal(EntryOptionComponent, { login: true });
  }

  public setReferralDetails(): MatDialogRef<FullscreenModalComponent, any> {
    return this.setModal(ReferralDetailsComponent);
  }

  public setChangePassword(): MatDialogRef<FullscreenModalComponent, any> {
    return this.setModal(ChangePasswordComponent);
  }

  public setEditAdvisor(advisorId: number): MatDialogRef<FullscreenModalComponent, any> {
    return this.setModal(AdvisorEditComponent, { id: advisorId });
  }

  public setConfirmDialog(title: string, message:string, closeLabel: string, confirmLabel?:string): MatDialogRef<ConfirmationDialogComponent, any> {
    var data = {
      title: title,
      message: message,
      closeLabel: closeLabel,
      confirmLabel: confirmLabel
    };
    return this.dialog.open(ConfirmationDialogComponent, {maxWidth: '375px', width:'calc(100% - 40px)', height: '180px', hasBackdrop: true, disableClose: true, panelClass:"confirm-dialog", data: data});
  }

  public setEditOrderDialog(order: OrderResponse): MatDialogRef<EditOrderComponent, any> {
    var data = {
      order: order
    };
    return this.dialog.open(EditOrderComponent, {maxWidth: '350px', width:'calc(100% - 40px)', height: '380px', hasBackdrop: true, disableClose: true, panelClass:"confirm-dialog", data: data});
  }

  public setNewOrderDialog(asset: AssetResponse): MatDialogRef<NewTradeWindowComponent, any> {
    var data = {
      asset: asset
    };
    return this.dialog.open(NewTradeWindowComponent, {maxWidth: '350px', width:'calc(100% - 40px)', height: '450px', hasBackdrop: true, disableClose: true, panelClass:"confirm-dialog", data: data});
  }

  public setEditStopLossDialog(order: OrderResponse): MatDialogRef<EditTradeValueComponent, any> {
    var data = {
      order: order,
      stopLossField: true,
      takeProfitField: false,
      amountField: false
    };
    return this.dialog.open(EditTradeValueComponent, {maxWidth: '370px', width:'calc(100% - 40px)',  height: '180px', hasBackdrop: true, disableClose: true, panelClass:"confirm-dialog", data: data});
  }

  public setEditTakeProfitDialog(order: OrderResponse): MatDialogRef<EditTradeValueComponent, any> {
    var data = {
      order: order,
      takeProfitField: true,
      stopLossField: false,
      amountField: false
    };
    return this.dialog.open(EditTradeValueComponent, {maxWidth: '370px', width:'calc(100% - 40px)', height: '180px', hasBackdrop: true, disableClose: true, panelClass:"confirm-dialog", data: data});
  }

  public setCloseOrderDialog(order: OrderResponse): MatDialogRef<EditTradeValueComponent, any> {
    var data = {
      order: order,
      amountField: true,
      takeProfitField: false,
      stopLossField: false
    };
    return this.dialog.open(EditTradeValueComponent, {maxWidth: '370px', width:'calc(100% - 40px)', height: '180px', hasBackdrop: true, disableClose: true, panelClass:"confirm-dialog", data: data});
  }
}
