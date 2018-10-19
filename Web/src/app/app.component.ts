import { Component, OnInit } from '@angular/core';
import { TopLoadingComponent } from './components/util/top-loading/top-loading.component';
import * as Highstock from 'highcharts/highstock.js';
import { Router } from '@angular/router';
import { NavigationService } from './services/navigation.service';
import { ModalService } from './services/modal.service';
import { AccountService } from './services/account.service';
import { ConfigService } from './services/config.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit{
  public topLoading = TopLoadingComponent.prototype.constructor;

  constructor(private router: Router, 
    private configService: ConfigService,
    private navigationService : NavigationService,
    private modalService: ModalService,
    private accountService : AccountService
    ) { }

  ngOnInit(){
  }

  

  public notificationOptions = {
    position: ["bottom", "left"],
    maxStack: 1,
    preventDuplicates: true,
    preventLastDuplicates: "visible",
    clickToClose: true,
    timeOut: 5000,
    pauseOnHover: true
  }

  public showEmptyPage() : boolean {
    return this.navigationService.isSameRoute("hotsite", this.router.url) ||
        this.navigationService.isSameRoute("beexpert", this.router.url) ||
        this.navigationService.isSameRoute("terminal", this.router.url);
  }

  openSocialMediaModal() {
    this.modalService.setInviteFriend();
  }

  isLogged(): boolean {
    if (this.accountService.isLoggedIn()) {
        let loginData = this.accountService.getLoginData();
        return !!loginData && loginData.hasInvestment;
    }
    return false;
  }
}