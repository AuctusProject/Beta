import { Component, OnInit } from '@angular/core';
import { AccountService } from '../../services/account.service';
import { LoginResponse } from '../../model/account/loginResponse';
import { FullscreenModalComponentInput } from '../../model/modal/fullscreenModalComponentInput';
import { LoginComponent } from '../account/login/login.component';
import { FullscreenModalComponent } from '../util/fullscreen-modal/fullscreen-modal.component';
import { MatDialog } from '@angular/material';
import { RegisterComponent } from '../account/register/register.component';
import { ChangePasswordComponent } from '../account/change-password/change-password.component';
import { ReferralDetailsComponent } from '../account/referral-details/referral-details.component';
import { AdvisorEditComponent } from '../advisor/advisor-edit/advisor-edit.component';

@Component({
  selector: 'header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.css']
})
export class HeaderComponent implements OnInit {
  loginData : LoginResponse;
  
  constructor(public dialog: MatDialog, 
    private accountService : AccountService) { }

  ngOnInit() {
    this.loginData = this.accountService.getLoginData();
  }

  isLogged(): boolean {
    this.loginData = this.accountService.getLoginData();
    return !!this.loginData;
  }

  logout() {
    this.accountService.logout();
    this.isLogged();
  }

  editAdvisor() {
    this.setModal(AdvisorEditComponent, { id: this.loginData.id });
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
