import { Injectable, NgZone } from '@angular/core';
import { Router } from '@angular/router';

@Injectable()
export class NavigationService {
  constructor(private router:Router, private zone : NgZone) {
  }

  public goToUrl(url: string, queryString?: any) {
    if (!!queryString) {
      this.zone.run(() => this.router.navigateByUrl(url, { queryParams: queryString }));
    } else {
      this.zone.run(() => this.router.navigateByUrl(url));
    }
  }

  public goToLogin(){
    this.goToUrl('', { login: true });
  }
  
  public goToBecomeAdvisor(){
    this.goToUrl('', { becomeadvisor: true });
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

  public goToExpertDetails(advisorId:number){
    this.goToUrl('expert-details/'+advisorId);
  }

  public goToAssetDetails(assetId:number){
    this.goToUrl('asset-details/'+assetId);
  }

  public goToHome(){
    this.goToUrl('');
  }

  public goToTopExperts(){
    this.goToUrl('top-experts');
  }
}
