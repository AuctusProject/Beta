import { Injectable } from '@angular/core';
import Web3 from 'web3';
import Web3Utils from 'web3-utils';
import { Observable } from 'rxjs';

declare let window: any;

@Injectable()
export class Web3Service {

  private web3: Web3;

  constructor() {
      

    
    // TODO: ACTIVATE EIP 1102 ON NOVEMBER 2ND   
    //     // If web3 is not injected (modern browsers)...
    // if (typeof window.web3 === 'undefined') {
    //     // Listen for provider injection
    //     window.addEventListener('message', ({ data }) => {
    //         if (data && data.type && data.type === 'ETHEREUM_PROVIDER_SUCCESS') {
    //             // Use injected provider, start dapp...
    //             window.web3 = new Web3(ethereum);
    //         }
    //     });
    //     // Request provider
    //     window.postMessage({ type: 'ETHEREUM_PROVIDER_REQUEST' }, '*');
    // }
    // // If web3 is injected (legacy browsers)...
    // else {
    //     // Use injected provider, start dapp
    //     this.web3 = new Web3(window.web3.currentProvider);
    // } 
   

    this.getWeb3().subscribe();
  }

  public getWeb3(): Observable<Web3> {
    let self = this;
    return new Observable (observer => {
      if (self.web3) {
        observer.next(self.web3);
      }
      else {
        window.addEventListener('load', function () {
          if (typeof window.web3 !== 'undefined') {
            self.web3 = new Web3(window.web3.currentProvider);
            observer.next(self.web3);
          }
          else {
            observer.next(null);
          }
        })
      }
    });
  }

  public getNetwork(): Observable<number> {
    let self = this;
    return new Observable(observer => {
      self.web3.version.getNetwork((err, netId) => {
        observer.next(netId);
      });
    })
  }

  public getAccount(): Observable<string> {
    let self = this;
    return new Observable(observer => {
      if (!self.web3) {
        observer.next(null);
      }
      else {
        self.web3.eth.getAccounts(function (err, accounts) {
          var currentAccount = accounts.length > 0 ? accounts[0] : null;
          observer.next(currentAccount);
        });
      }
    });
  }

  public toHex(val: string): string {
    return Web3Utils.toHex(val);
  }

  public toWei(value: string, unit?: string) {
    return Web3Utils.toWei(value, unit);
  }
  
}
