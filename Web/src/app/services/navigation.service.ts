import { Injectable, NgZone } from '@angular/core';
import { Router } from '@angular/router';

@Injectable()
export class NavigationService {
  constructor(private router:Router, private zone : NgZone) {
  }

  public goToUrl(url:string){
    this.zone.run(() => this.router.navigateByUrl(url));
  }

  public goToLogin(){
    this.goToUrl('login');
  }
  
  public goToBecomeAdvisor(){
    this.goToUrl('become-advisor');
  }

  public goToWalletLogin(){
    this.goToUrl('wallet-login');
  }

  public goToFeed(){
    this.goToUrl('feed');
  }

  public goToConfirmEmail(){
    this.goToUrl('confirm-email');
  }

  public goToAdvisorDetails(advisorId:number){
    this.goToUrl('advisor-details/'+advisorId);
  }

  public goToAssetDetails(assetId:number){
    this.goToUrl('asset-details/'+assetId);
  }

  public goToHome(){
    this.goToUrl('home');
  }
}
