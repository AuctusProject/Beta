import { Injectable } from '@angular/core';
import { Router } from '@angular/router';

@Injectable()
export class NavigationService {
  constructor(private router:Router) {
  }

  public goToUrl(url:string){
    this.router.navigateByUrl(url);
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

  public goToAdvisorDetail(advisorId:number){
    this.goToUrl('advisor/'+advisorId);
  }

  public goToHome(){
    this.goToUrl('home');
  }
}
