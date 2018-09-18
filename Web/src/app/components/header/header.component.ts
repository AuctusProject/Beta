import { Component, OnInit, ViewChild } from '@angular/core';
import { AccountService } from '../../services/account.service';
import { LoginResponse } from '../../model/account/loginResponse';
import { MatMenu } from '@angular/material';
import { NavigationService } from '../../services/navigation.service';
import { ModalService } from '../../services/modal.service';

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
  
  constructor(private modalService: ModalService, 
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

  onBecomeExpert() {
    if (this.isLogged()) {
      this.modalService.setBecomeAdvisorForm();
    } else {
      this.modalService.setBecomeAdvisor();
    }
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

  advisorDetails() {
    this.navigationService.goToExpertDetails(this.loginData.id);
  }

  editAdvisor() {
    this.modalService.setEditAdvisor(this.loginData.id);
  }

  configuration() {
    this.modalService.setConfiguration();
  }

  referralDetails() {
    this.modalService.setReferralDetails();
  }

  changePassword() {
    this.modalService.setChangePassword();
  }

  login() {
    this.modalService.setLogin();
  }

  register() {
    this.modalService.setRegister();
  }
}
