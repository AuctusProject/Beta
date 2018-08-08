import { Injectable } from '@angular/core';
import { Router, Route, CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { AccountService } from '../services/account.service';
import { LoginData } from '../model/account/loginData';

@Injectable()
export class AuthRedirect implements CanActivate {

  constructor(private accountService: AccountService, private router: Router) { }

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
    let loginData = this.accountService.getLoginData();
    if (loginData) {
      var currentUrl = state.url;
      return !this.redirect(loginData, currentUrl);
    }
    else{
      this.router.navigateByUrl('login');
    }
    return false;
  }

  redirectAfterLoginAction(){
    let loginData = this.accountService.getLoginData();
    if (loginData) {
      let redirected = this.redirect(loginData)
      if(!redirected){
        this.router.navigateByUrl('feed');
      }
    }
  }

  private redirect(loginData: LoginData, currentUrl?: string):boolean{
    if(loginData.pendingConfirmation){
      if(currentUrl == null || !currentUrl.startsWith('/confirm-email')) {
        this.router.navigateByUrl('confirm-email');
        return true;
      }
    }
    else if(loginData.requestedToBeAdvisor && !loginData.isAdvisor){
      if(currentUrl != '/advisor-details') {
        this.router.navigateByUrl('advisor-details');
        return true;
      }
    }    
    else if(!loginData.isAdvisor && !loginData.hasInvestment){
      if(currentUrl != '/wallet-login') {
        this.router.navigateByUrl('wallet-login');
        return true;
      }
    }
    else if(loginData.isAdvisor){
      if (currentUrl != '/advisor/' + loginData.id) {
        this.router.navigateByUrl('advisor/' + loginData.id);
        return true;
      }
    }
    return false;
  }
}
