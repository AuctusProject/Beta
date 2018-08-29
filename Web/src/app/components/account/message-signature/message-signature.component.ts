import { Component, OnInit } from '@angular/core';
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
export class MessageSignatureComponent implements OnInit {
  hasMetamask : boolean = true;
  constructor(private web3Service : Web3Service, 
    private navigationService: NavigationService,
    private accountService : AccountService,
    private authRedirect: AuthRedirect) { }

  ngOnInit() {
  }

  signMessage(){
    this.web3Service.getAccount().subscribe(account => 
      this.sendMessage(account)
    );
  }

  sendMessage(account){
    var message = (Constants.signatureMessage);
    this.web3Service.getWeb3().subscribe(web3 => web3.currentProvider.sendAsync({
      jsonrpc: "2.0",
      method: "personal_sign",
      params: [this.web3Service.toHex(message), account]
    },(a,signatureInfo)=>{
      this.handleSignatureResult(account, signatureInfo);      
    }));
  }

  handleSignatureResult(account, signatureInfo){
    if(signatureInfo.result){
      var validateSignatureRequest = new ValidateSignatureRequest();
      validateSignatureRequest.address = account;
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
