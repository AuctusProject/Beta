import { Component, OnInit } from '@angular/core';
import { Web3Service } from '../../../services/web3.service';
import { AccountService } from '../../../services/account.service';
import { ValidateSignatureRequest } from '../../../model/account/validateSignatureRequest';

@Component({
  selector: 'message-signature',
  templateUrl: './message-signature.component.html',
  styleUrls: ['./message-signature.component.css']
})
export class MessageSignatureComponent implements OnInit {
  hasMetamask : boolean = true;
  constructor(private web3Service : Web3Service, private accountService : AccountService) { }

  ngOnInit() {
  }

  signMessage(){
    this.web3Service.getWeb3().subscribe(web3 => web3.currentProvider.sendAsync({
      id: "1c8193c2b2424f78ce880aaf389e9f12",
      jsonrpc: "2.0",
      method: "personal_sign",
      params: [this.web3Service.toHex("teste"),"0xb4610ea94f4a7769979b33933ec0761d9ae0446b"]
    },(a,signatureInfo)=>{
      console.log(signatureInfo);
      if(signatureInfo.result){
        this.web3Service.getAccount().subscribe(account => {
          var validateSignatureRequest = new ValidateSignatureRequest();
          validateSignatureRequest.address = account;
          validateSignatureRequest.signature = signatureInfo.result;
          this.accountService.validateSignature(validateSignatureRequest).subscribe(loginResponse => console.log(loginResponse));
        });
        alert(signatureInfo.result);
      }
      else if(signatureInfo.error && signatureInfo.error.message){
        alert(signatureInfo.error.message);
      }
      else{
        alert("Error signing message");
      }
    }));
  }

}
