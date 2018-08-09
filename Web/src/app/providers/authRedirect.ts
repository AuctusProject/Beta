import { Injectable } from '@angular/core';
import { Router, Route, CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { AccountService } from '../services/account.service';
import { LoginResponse} from '../model/account/loginResponse';

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

  private redirect(loginResponse: LoginResponse, currentUrl?: string):boolean{
    if(loginResponse.pendingConfirmation){
      if(currentUrl == null || !currentUrl.startsWith('/confirm-email')) {
        this.router.navigateByUrl('confirm-email');
        return true;
      }
    }
    else if(loginResponse.requestedToBeAdvisor && !loginResponse.isAdvisor){
      if(currentUrl != '/advisor-details') {
        this.router.navigateByUrl('advisor-details');
        return true;
      }
    }    
    else if(!loginResponse.isAdvisor && !loginResponse.hasInvestment){
      if(currentUrl != '/wallet-login') {
        this.router.navigateByUrl('wallet-login');
        return true;
      }
    }
    else if(loginResponse.isAdvisor){
      if (currentUrl != '/advisor/' + loginResponse.id) {
        this.router.navigateByUrl('advisor/' + loginResponse.id);
        return true;
      }
    }
    return false;
  }
}
