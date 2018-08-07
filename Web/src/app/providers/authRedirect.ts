import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { AccountService } from '../services/account.service';
import { ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router/src/router_state';

@Injectable()
export class AuthRedirect implements CanActivate {

  constructor(private accountService: AccountService, private router: Router) { }

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
    let logged = this.accountService.isLoggedIn();
    if (logged) {
      let loginData = this.accountService.getLoginData();
      if (loginData) {
        if (!loginData.hasInvestment && loginData.isAdvisor) {
          this.router.navigateByUrl('advisor/' + loginData.id);
        } else {
          this.router.navigateByUrl('feed');
        }
      }
      else {
        return true;
      }
    }
    return !logged;
  }
}
