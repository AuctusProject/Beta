import { Component, OnInit, ViewChild } from '@angular/core';
import { AccountService } from '../../services/account.service';
import { LoginResponse } from '../../model/account/loginResponse';
import { FullscreenModalComponentInput } from '../../model/modal/fullscreenModalComponentInput';
import { LoginComponent } from '../account/login/login.component';
import { FullscreenModalComponent } from '../util/fullscreen-modal/fullscreen-modal.component';
import { MatDialog, MatMenu } from '@angular/material';
import { RegisterComponent } from '../account/register/register.component';
import { ChangePasswordComponent } from '../account/change-password/change-password.component';
import { ReferralDetailsComponent } from '../account/referral-details/referral-details.component';
import { AdvisorEditComponent } from '../advisor/advisor-edit/advisor-edit.component';
import { NavigationService } from '../../services/navigation.service';
import { ConfigurationComponent } from '../account/configuration/configuration.component';

@Component({
  selector: 'header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.css']
})
export class HeaderComponent implements OnInit {
  loginData: LoginResponse;
  menuOpen: boolean = false;
  @ViewChild("profile") profile: MatMenu;
  @ViewChild("mobile") mobile: MatMenu;
  
  constructor(public dialog: MatDialog, 
    private accountService : AccountService, 
    private navigationService: NavigationService) { }

  ngOnInit() {
    this.loginData = this.accountService.getLoginData();
    if (!!this.profile) {
      this.profile.closed.subscribe(() => this.menuOpen = false);
    }
    if (!!this.mobile) {
      this.mobile.closed.subscribe(() => this.menuOpen = false);
    }
  }

  isLogged(): boolean {
    this.loginData = this.accountService.getLoginData();
    return !!this.loginData;
  }

  onBecameExpert() {
    //TODO navigation
  }

  onTopExperts() {
    this.navigationService.goToTopExperts();
  }

  onTopAssets() {
    this.navigationService.goToTopAssets();
  }

  logout() {
    this.accountService.logout();
    this.isLogged();
    this.navigationService.goToHome();
  }

  editAdvisor() {
    this.setModal(AdvisorEditComponent, { id: this.loginData.id });
  }

  configuration() {
    this.setModal(ConfigurationComponent);
  }

  referralDetails() {
    this.setModal(ReferralDetailsComponent);
  }

  changePassword() {
    this.setModal(ChangePasswordComponent);
  }

  login() {
    this.setModal(LoginComponent);
  }

  register() {
    this.setModal(RegisterComponent);
  }

  setModal(modal: any, data?: any) {
    let modalData = new FullscreenModalComponentInput();
    modalData.component = modal;
    modalData.componentInput = data;
    const dialogRef = this.dialog.open(FullscreenModalComponent, { data: modalData }); 

    dialogRef.afterClosed().subscribe(result => { this.isLogged(); });
  }
}
