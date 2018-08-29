import { Component, OnInit, ChangeDetectorRef, OnDestroy } from '@angular/core';
import { Web3Service } from '../../../services/web3.service';
import { AccountService } from '../../../services/account.service';
import { ValidateSignatureRequest } from '../../../model/account/validateSignatureRequest';
import { Constants } from '../../../util/constants';
import { AuthRedirect } from '../../../providers/authRedirect';
import { NavigationService } from '../../../services/navigation.service';

@Component({
  selector: 'message-signature',
  templateUrl: './message-signature.component.html',
  styleUrls: ['./message-signature.component.css']
})
export class MessageSignatureComponent implements OnInit, OnDestroy {
  hasMetamask : boolean;
  hasUnlockedAccount : boolean;
  account: string;
  accountInterval;
  constructor(private web3Service : Web3Service, 
    private navigationService: NavigationService,
    private accountService : AccountService,
    private authRedirect: AuthRedirect,
    private changeDetector: ChangeDetectorRef) { }

  ngOnInit() {
    this.monitorMetamaskAccount();
  }

  ngOnDestroy(){
    clearInterval(this.accountInterval);
  }

  private monitorMetamaskAccount() {
    let self = this;
    this.accountInterval = setInterval(function () {
      self.web3Service.getWeb3().subscribe(result => 
        {
          if(result){
            self.hasMetamask = true;
            self.changeDetector.detectChanges();
            self.web3Service.getAccount().subscribe(
              account => {
                if (account) {
                  self.hasUnlockedAccount = true;
                }
                else{
                  self.hasUnlockedAccount = false;
                }
                self.account = account;
                self.changeDetector.detectChanges();
              })
          }
          else{
            self.hasMetamask = false;
          }
        }
      )}, 100);
  }

  signMessage(){
    this.sendMessage();
  }

  private sendMessage(){
    var message = (Constants.signatureMessage);
    this.web3Service.getWeb3().subscribe(web3 => web3.currentProvider.sendAsync({
      jsonrpc: "2.0",
      method: "personal_sign",
      params: [this.web3Service.toHex(message), this.account]
    },(a,signatureInfo)=>{
      this.handleSignatureResult(signatureInfo);      
    }));
  }

  handleSignatureResult(signatureInfo){
    if(signatureInfo.result){
      var validateSignatureRequest = new ValidateSignatureRequest();
      validateSignatureRequest.address = this.account;
      validateSignatureRequest.signature = signatureInfo.result;
      this.accountService.validateSignature(validateSignatureRequest).subscribe(result =>
        {
          this.accountService.setLoginData(result);
          this.authRedirect.redirectAfterLoginAction();
        }
      )
    }
    else if(signatureInfo.error && signatureInfo.error.message){
      alert(signatureInfo.error.message);
    }
    else{
      alert("Error signing message");
    }
  }

  becomeAdvisor(){
    this.navigationService.goToBecomeAdvisor();
  }
}
