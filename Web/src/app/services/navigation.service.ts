import { Injectable, NgZone } from '@angular/core';
import { Router } from '@angular/router';

@Injectable()
export class NavigationService {
  constructor(private router: Router, private zone : NgZone) {
  }

  public goToUrl(url: string, queryString?: any) {
    if (!!queryString) {
      let reloadPage = this.isSameRoute(url);
      this.zone.run(() => this.router.navigate([url], { queryParams: queryString }).then(result => 
      {
        if (reloadPage) {
          window.location.reload();
        }
      }));
    } else {
      this.zone.run(() => this.router.navigateByUrl(url));
    }
  }

  public isSameRoute(url: string, currentUrl?: string): boolean {
    if (currentUrl === undefined || currentUrl === null) {
      currentUrl = this.router.url;
    }
    let currentRoute = currentUrl.split("?")[0];
    if (currentRoute && currentRoute.length > 0 && currentRoute[0] === "/") {
      currentRoute = currentRoute.slice(1);
    }
    if (url && url.length > 0 && url[0] === "/") {
      url = url.slice(1);
    }
    return url.toLowerCase() === currentRoute.toLowerCase();
  }

  public goToLogin(){
    this.goToUrl('', { login: true });
  }

  public goToCompleteRegistration(){
    this.goToUrl('', { completeregistration: true });
  }

  public goToConfirmEmail(){
    this.goToUrl('', { confirmemail: true });
  }

  public goToExpertDetails(advisorId:number){
    this.goToUrl('top-traders/'+advisorId);
  }

  public goToAssetDetails(assetId:number){
    this.goToUrl('trade-markets/'+assetId);
  }

  public goToHome(){
    this.goToUrl('');
  }

  public goToTopExperts(){
    this.goToUrl('top-traders');
  }

  public goToTradeMarkets(){
    this.goToUrl('trade-markets');
  }

  public goToPortfolio(){
    this.goToUrl('portfolio');
  }
}
