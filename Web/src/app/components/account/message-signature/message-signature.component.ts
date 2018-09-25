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
import { Subscription } from 'rxjs';
import { ModalService } from '../../../services/modal.service';
import { CONFIG } from '../../../services/config.service';

@Component({
  selector: 'message-signature',
  templateUrl: './message-signature.component.html',
  styleUrls: ['./message-signature.component.css']
})
export class MessageSignatureComponent implements OnInit, OnDestroy {
  hasMetamask: boolean = false;
  hasUnlockedAccount: boolean = false;
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
  promise: Subscription;

  constructor(private web3Service : Web3Service, 
    private navigationService: NavigationService,
    private accountService : AccountService,
    private localStorageService : LocalStorageService,
    private authRedirect: AuthRedirect,
    private notificationsService: NotificationsService,
    private changeDetector: ChangeDetectorRef,
    private modalService: ModalService) { }

  ngOnInit() {
    this.checkMetamask();
    this.setBancorWidget();
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
    let loginData = this.accountService.getLoginData();
    if (loginData && loginData.requestedToBeAdvisor && !loginData.hasInvestment && !loginData.isAdvisor) {
      this.modalService.setBecomeAdvisor();
    }
  }

  getTopTitle() : string {
    return "HELLO";
  }

  getTopImage() : string {
    return CONFIG.platformImgUrl.replace("{id}", "feed1920px");
  }

  getTopText() : string {
    return "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.";
  }

  shouldShowBancorWidget() {
    return window && window["BancorConvertWidget"] && window["BancorConvertWidget"].isInitialized;
  }

  setBancorWidget() {
    if (window && window["BancorConvertWidget"]) {
      if (this.isMobile() || this.hasAuc()) {
        if (window["BancorConvertWidget"].isInitialized) {
          window["BancorConvertWidget"].deinit();
        }
      } else if (!window["BancorConvertWidget"].isInitialized &&
                  (!this.hasAuc() && this.hasMetamask && this.hasUnlockedAccount) ) {
        window["BancorConvertWidget"].init({
          "type": "2",
          "blockchainType": "ethereum",
          "baseCurrencyId": "5ad9c1d54c4998a2f940e933",
          "pairCurrencyId": "5937d635231e97001f744267",
          "primaryColor": "#126efd",
          "displayCurrency": "ETH",
          "hideVolume": true
        });
      }
    }
  }

  isMobile() {
    return window.screen.width <= 600;
  }

  ngOnDestroy(){
    if (this.timer) {
      clearTimeout(this.timer);
    }
    if (window && window["BancorConvertWidget"] && window["BancorConvertWidget"].isInitialized) {
      window["BancorConvertWidget"].deinit();
    }
  }

  getImgSrc(imageName: string) {
   return CONFIG.platformImgUrl.replace("{id}", imageName);
  }

  checkMetamask() {
    let self = this;
    this.web3Service.getWeb3().subscribe(result => 
      {
        if(result) {
          self.hasMetamask = true;
          self.changeDetector.detectChanges();
          self.setBancorWidget();
          self.web3Service.getAccount().subscribe(
            account => {
              self.account = account;
              if (account) {
                self.hasUnlockedAccount = true;
                self.checkAUCAmout();
              } else {
                self.hasUnlockedAccount = false;
                self.aucAmount = 0;
              }
              self.changeDetector.detectChanges();
            });
        } else {
          self.hasMetamask = false;
          self.hasUnlockedAccount = false;
          self.aucAmount = 0;
        }
        self.setBancorWidget();
        self.timer = setTimeout(() => self.checkMetamask(), 1000);
      });
  }

  checkAUCAmout() {
    if (this.account) {
      if (this.account != this.lastAccountChecked || 
          (!this.hasAuc() && !!this.lastCheck && ((new Date()) >= (new Date(this.lastCheck.getTime() + 15000))))) {
        this.accountService.getAUCAmount(this.account).subscribe(ret => 
          {
            this.lastCheck = new Date();
            this.lastAccountChecked = this.account;
            this.aucAmount = ret;
            this.setBancorWidget();
          });
      }
    } else {
      this.aucAmount = 0;
    }
  }

  hasAuc() {
    return this.aucAmount >= this.aucRequired && (this.aucAmount > 0 || this.aucRequired == 0);
  }

  signMessage() {
    if (this.hasMetamask && this.hasUnlockedAccount && this.hasAuc()) {
      var message = (Constants.signatureMessage);
      this.promise = this.web3Service.getWeb3().subscribe(web3 => web3.currentProvider.sendAsync({
        jsonrpc: "2.0",
        method: "personal_sign",
        params: [this.web3Service.toHex(message), this.account]
      }, (a,signatureInfo) => {
        this.handleSignatureResult(signatureInfo);      
      }));
    }
  }

  handleSignatureResult(signatureInfo) {
    if(signatureInfo.result) {
      var validateSignatureRequest = new ValidateSignatureRequest();
      validateSignatureRequest.address = this.account;
      validateSignatureRequest.signature = signatureInfo.result;
      this.promise = this.accountService.validateSignature(validateSignatureRequest).subscribe(result =>
        {
          this.accountService.setLoginData(result);
          this.authRedirect.redirectAfterLoginAction(result);
        }
      );
    } else if (signatureInfo.error && signatureInfo.error.message) {
      this.notificationsService.error(signatureInfo.error.message);
    } else {
      this.notificationsService.error("Error signing message");
    }
  }

  becomeAdvisor() {
    this.navigationService.goToBecomeAdvisor()
  }

  getReferralOptions() {
    return { darkLayout: true, textOptions: { outlineField: false, placeHolder: "Type in your discount code...", required: false, showHintSize: false, minLength: 7, maxLength: 7} };
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
    this.discountMessage = "Congratulations, using the referral code you need to hold " + discount + "% less AUC on your wallet!" 
  }

  missingAmountMessage() {
    if (!this.hasAuc() && this.aucAmount > 0) {
      return "Missing " + (this.aucRequired - this.aucAmount) + " AUC on your wallet." 
    } else {
      return "";
    }
  }

  setInvalidReferral(message: string) {
    this.Referral.setForcedError(message);
    this.discountMessage = "";
  }
}
