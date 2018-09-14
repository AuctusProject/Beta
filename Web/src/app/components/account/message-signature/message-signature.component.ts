import { Component, OnInit, ChangeDetectorRef, OnDestroy, ViewChild } from '@angular/core';
import { Web3Service } from '../../../services/web3.service';
import { AccountService } from '../../../services/account.service';
import { ValidateSignatureRequest } from '../../../model/account/validateSignatureRequest';
import { Constants } from '../../../util/constants';
import { AuthRedirect } from '../../../providers/authRedirect';
import { NavigationService } from '../../../services/navigation.service';
import { NotificationsService } from 'angular2-notifications';
import { LocalStorageService } from '../../../services/local-storage.service';
import { InheritanceInputComponent } from '../../util/inheritance-input/inheritance-input.component';

@Component({
  selector: 'message-signature',
  templateUrl: './message-signature.component.html',
  styleUrls: ['./message-signature.component.css']
})
export class MessageSignatureComponent implements OnInit, OnDestroy {
  hasMetamask: boolean = false;
  hasUnlockedAccount: boolean = false;
  hasAUC: boolean = false;
  showReferral: boolean = true;
  account: string;
  lastAccountChecked: string;
  lastCheck: Date;
  timer: any;
  referralCode: string;
  discountMessage: string = "";
  standardAUCAmount: number;
  aucRequired: number;
  aucAmount: number = 0;
  @ViewChild("Referral") Referral: InheritanceInputComponent;

  constructor(private web3Service : Web3Service, 
    private navigationService: NavigationService,
    private accountService : AccountService,
    private localStorageService : LocalStorageService,
    private authRedirect: AuthRedirect,
    private notificationsService: NotificationsService,
    private changeDetector: ChangeDetectorRef) { }

  ngOnInit() {
    this.checkMetamask();
    this.accountService.getWalletLoginInfo().subscribe(result =>
      {
        this.showReferral = !result.registeredWallet;
        this.standardAUCAmount = result.standardAUCAmount;
        this.aucRequired = result.aucRequired;
        if (!result.referralCode) {
          this.referralCode = this.localStorageService.getLocalStorage("referralCode");
          if (this.referralCode) {
            this.validateReferralCode(this.referralCode);
          }
        } else {
          this.referralCode = result.referralCode;
        }
        if (result.discount > 0) {
          this.setDiscountMessage(result.discount);
        }
      });
  }

  ngOnDestroy(){
    if (this.timer) {
      clearTimeout(this.timer);
    }
  }

  checkMetamask() {
    let self = this;
    this.web3Service.getWeb3().subscribe(result => 
      {
        if(result) {
          self.hasMetamask = true;
          self.changeDetector.detectChanges();
          self.web3Service.getAccount().subscribe(
            account => {
              self.account = account;
              if (account) {
                self.hasUnlockedAccount = true;
                this.checkAUCAmout();
              } else {
                self.hasUnlockedAccount = false;
                self.hasAUC = false;
              }
              self.changeDetector.detectChanges();
            })
        } else {
          self.hasMetamask = false;
        }
        self.timer = setTimeout(() => self.checkMetamask(), 1000);
      });
  }

  checkAUCAmout() {
    if (this.account) {
      if (this.account != this.lastAccountChecked || 
          (!this.hasAUC && !!this.lastCheck && ((new Date()) <= (new Date(this.lastCheck.getTime() + 20000))))) {
        this.accountService.getAUCAmount(this.account).subscribe(ret => 
          {
            this.lastCheck = new Date();
            this.lastAccountChecked = this.account;
            this.aucAmount = ret;
            this.hasAUC = ret >= this.aucRequired;
          });
      }
    } else {
      this.hasAUC = false;
    }
  }

  signMessage() {
    var message = (Constants.signatureMessage);
    this.web3Service.getWeb3().subscribe(web3 => web3.currentProvider.sendAsync({
      jsonrpc: "2.0",
      method: "personal_sign",
      params: [this.web3Service.toHex(message), this.account]
    }, (a,signatureInfo) => {
      this.handleSignatureResult(signatureInfo);      
    }));
  }

  handleSignatureResult(signatureInfo) {
    if(signatureInfo.result) {
      var validateSignatureRequest = new ValidateSignatureRequest();
      validateSignatureRequest.address = this.account;
      validateSignatureRequest.signature = signatureInfo.result;
      this.accountService.validateSignature(validateSignatureRequest).subscribe(result =>
        {
          this.accountService.setLoginData(result);
          this.authRedirect.redirectAfterLoginAction();
        }
      );
    } else if (signatureInfo.error && signatureInfo.error.message) {
      this.notificationsService.error(signatureInfo.error.message);
    } else {
      this.notificationsService.error("Error signing message");
    }
  }

  becomeAdvisor() {
    this.navigationService.goToBecomeAdvisor();
  }

  getReferralOptions() {
    return { darkLayout: true, textOptions: { outlineField: false, placeHolder: "Referral code (optional)", required: false, showHintSize: false, minLength: 7, maxLength: 7 } };
  }

  onChangeReferralCode(value: string) {
    this.referralCode = value;
    this.validateReferralCode(value);
  }

  validateReferralCode(value: string) {
    this.accountService.setReferralCode(value).subscribe(response => {
      this.standardAUCAmount = response.standardAUCAmount;
      this.aucRequired = response.aucRequired;
      if (response.valid) {
        this.Referral.setForcedError("");
        this.setDiscountMessage(response.discount); 
      } else if (!!value && value.length > 0) {
        this.setInvalidReferral("Invalid referral code");
      } else {
        this.setInvalidReferral("");
      }
    });
  }

  setDiscountMessage(discount: number) {
    this.discountMessage = "Congratulations, using the referral code you need hold " + discount + "% less AUC in your own wallet!" 
  }

  missingAmountMessage() {
    if (!this.hasAUC && this.aucAmount > 0) {
      return "Missing " + (this.aucRequired - this.aucAmount) + " AUC in your wallet." 
    } else {
      return "";
    }
  }

  setInvalidReferral(message: string) {
    this.Referral.setForcedError(message);
    this.discountMessage = "";
  }
}
