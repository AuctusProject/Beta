import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { AccountService } from '../services/account.service';
import { LoginResponse} from '../model/account/loginResponse';
import { LocalStorageService } from '../services/local-storage.service';
import { NavigationService } from '../services/navigation.service';

@Injectable()
export class AuthRedirect implements CanActivate {

  constructor(private accountService: AccountService, private navigationService: NavigationService, private localStorageService: LocalStorageService) { }

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
    let loginData = this.accountService.getLoginData();
    if (loginData) {
      var currentUrl = state.url;
      return !this.redirect(loginData, currentUrl);
    }
    else{
      this.redirectToLogin(state.url);
    }
    return false;
  }

  redirectAfterLoginAction(){
    let loginData = this.accountService.getLoginData();
    if (loginData) {
      let redirected = this.redirect(loginData)
      if(!redirected){
        var redirectUrl = this.localStorageService.getLocalStorage("redirectUrl");
        this.localStorageService.removeLocalStorage("redirectUrl");
        if(redirectUrl)
          this.navigationService.goToUrl(redirectUrl);
        else
          this.navigationService.goToFeed();
      }
    }
  }

  redirectToLogin(currentUrl?: string){
    if(currentUrl){
      this.localStorageService.setLocalStorage("redirectUrl", currentUrl);
    }
    this.navigationService.goToLogin();
  }

  private redirect(loginResponse: LoginResponse, currentUrl?: string):boolean{
    if(loginResponse.pendingConfirmation){
      if(currentUrl == null || !currentUrl.startsWith('/confirm-email')) {
        this.navigationService.goToConfirmEmail();
        return true;
      }
    }
    else if (loginResponse.requestedToBeAdvisor && !loginResponse.isAdvisor){
      if(currentUrl != '/become-advisor' && currentUrl != '/wallet-login') {
        this.navigationService.goToBecomeAdvisor();
        return true;
      }
    }    
    else if (!loginResponse.isAdvisor && !loginResponse.hasInvestment){
      if(currentUrl != '/wallet-login' && currentUrl != '/become-advisor') {
        this.navigationService.goToWalletLogin();
        return true;
      }
    }
    else if(loginResponse.isAdvisor){
      if (currentUrl != '/advisor/' + loginResponse.id) {
        this.navigationService.goToAdvisorDetail(loginResponse.id);
        return true;
      }
    }
    return false;
  }
}
