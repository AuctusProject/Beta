import { Injectable, NgZone } from '@angular/core';
import { Router } from '@angular/router';

@Injectable()
export class NavigationService {
  constructor(private router: Router, private zone : NgZone) {
  }

  public goToUrl(url: string, queryString?: any) {
    if (!!queryString) {
      let reloadPage = this.isSameRoute(url);
      this.zone.run(() => this.router.navigate([url], { queryParams: queryString, queryParamsHandling: "merge" }).then(result => 
      {
        if (reloadPage) {
          window.location.reload();
        }
      }));
    } else {
      this.zone.run(() => this.router.navigateByUrl(url));
    }
  }

  private isSameRoute(url: string): boolean {
    let currentRoute = this.router.url.split("?")[0];
    if (currentRoute && currentRoute.length > 0 && currentRoute[0] === "/") {
      currentRoute = currentRoute.slice(1);
    }
    if (url && url.length > 0 && url[0] === "/") {
      url = url.slice(1);
    }
    return url === currentRoute;
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
    this.goToUrl('', { confirmemail: true });
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

  public goToTopAssets(){
    this.goToUrl('top-assets');
  }
}
