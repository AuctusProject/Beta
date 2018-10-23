import { Injectable, PLATFORM_ID, Inject } from '@angular/core';
import { FullscreenModalComponent } from '../components/util/fullscreen-modal/fullscreen-modal.component';
import { FullscreenModalComponentInput } from '../model/modal/fullscreenModalComponentInput';
import { MatDialog, MatDialogRef } from '@angular/material';
import { ConfirmEmailComponent } from '../components/account/confirm-email/confirm-email.component';
import { ConfigurationComponent } from '../components/account/configuration/configuration.component';
import { EntryOptionComponent } from '../components/account/entry-option/entry-option.component';
import { ForgotPasswordResetComponent } from '../components/account/forgot-password-reset/forgot-password-reset.component';
import { BecomeAdvisorComponent } from '../components/advisor/become-advisor/become-advisor.component';
import { ChangePasswordComponent } from '../components/account/change-password/change-password.component';
import { ReferralDetailsComponent } from '../components/account/referral-details/referral-details.component';
import { AdvisorEditComponent } from '../components/advisor/advisor-edit/advisor-edit.component';
import { NewAdviceComponent } from '../components/advisor/new-advice/new-advice.component';
import { InviteFriendComponent } from '../components/account/invite-friend/invite-friend.component';
import { isPlatformBrowser } from '@angular/common';

@Injectable()
export class ModalService {
  constructor(
    @Inject(PLATFORM_ID) private platformId: Object,
    private dialog: MatDialog) {
  }

  private setModal(component: any, componentInputData?: any, hiddenClose: boolean = false): MatDialogRef<FullscreenModalComponent, any> {
    if(isPlatformBrowser(this.platformId)){
      let modalData = new FullscreenModalComponentInput();
      modalData.component = component;
      modalData.componentInput = componentInputData;
      modalData.hiddenClose = hiddenClose;
      return this.dialog.open(FullscreenModalComponent, { data: modalData }); 
    }
    return null;
  }

  public setConfirmEmail(): MatDialogRef<FullscreenModalComponent, any> {
    return this.setModal(ConfirmEmailComponent);
  }

  public setConfiguration(): MatDialogRef<FullscreenModalComponent, any> {
    return this.setModal(ConfigurationComponent);
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

  public setNewAdvice(assetId?: number, adviceType? :number): MatDialogRef<FullscreenModalComponent, any> {
    return this.setModal(NewAdviceComponent, { assetId: assetId, adviceType: adviceType });
  }

  public setInviteFriend(): MatDialogRef<FullscreenModalComponent, any> {
    return this.setModal(InviteFriendComponent);
  }
}
