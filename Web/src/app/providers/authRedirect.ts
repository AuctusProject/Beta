import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { AccountService } from '../services/account.service';
import { LoginResponse} from '../model/account/loginResponse';
import { LocalStorageService } from '../services/local-storage.service';
import { NavigationService } from '../services/navigation.service';
import { Observable } from 'rxjs';

@Injectable()
export class AuthRedirect implements CanActivate {

  constructor(private accountService: AccountService, private navigationService: NavigationService, private localStorageService: LocalStorageService) { }

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) : Observable<boolean> {
    let loginData = this.accountService.getLoginData();
    if (!!loginData) {
      return new Observable (observer => 
        {
          this.redirect(loginData, state.url).subscribe(ret => observer.next(!ret));
        });
    } else {
      this.redirectToLogin(state.url);
    }
    return new Observable (observer => observer.next(false));
  }

  redirectAfterLoginAction(loginResponse?: LoginResponse) {
    let loginData;
    if (!!loginResponse) {
      loginData = loginResponse;
    } else {
      loginData = this.accountService.getLoginData();
    }
    if (!!loginData) {
      this.redirect(loginData).subscribe(redirected =>
        {
          if (!redirected) {
            var redirectUrl = this.localStorageService.getLocalStorage("redirectUrl");
            this.localStorageService.removeLocalStorage("redirectUrl");
            if (redirectUrl) {
              this.navigationService.goToUrl(redirectUrl);
            } else {
              this.redirectToHome(this.accountService.getLoginData());
            }
          }
        });
    }
  }

  redirectToLogin(currentUrl?: string) {
    if (currentUrl) {
      this.localStorageService.setLocalStorage("redirectUrl", currentUrl);
    }
    this.navigationService.goToLogin();
  }

  private redirect(loginResponse: LoginResponse, currentUrl?: string) : Observable<boolean> {
    if(this.navigationService.isSameRoute('wallet-login', currentUrl)) {
      return new Observable (observer => observer.next(false));
    } else if ((!loginResponse.hasInvestment && !loginResponse.isAdvisor) || loginResponse.pendingConfirmation) {
        return new Observable (observer => 
          {
            this.accountService.getUserData().subscribe(ret =>
            {
              this.accountService.setLoginData(ret);
              if (!ret.isAdvisor && !loginResponse.hasInvestment) {
                this.navigationService.goToWalletLogin();
                return observer.next(true);
              } else if(ret.pendingConfirmation) {
                this.navigationService.goToConfirmEmail();
                return observer.next(true);
              } else {
                return observer.next(false);
              }
            }, () => observer.next(false));
          });
    } else {
      return new Observable (observer => observer.next(false));
    }
  }

  redirectToHome(loginResponse: LoginResponse) {
    if(loginResponse.isAdvisor) {
      this.navigationService.goToExpertDetails(loginResponse.id);
    } else {
      this.navigationService.goToFeed();
    }
  }
}
