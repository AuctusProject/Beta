import { Component, OnInit } from '@angular/core';
import { LoginComponent } from '../../account/login/login.component';
import { MatDialog } from '@angular/material';
import { FullscreenModalComponent } from '../../util/fullscreen-modal/fullscreen-modal.component';
import { FullscreenModalComponentInput } from '../../../model/modal/fullscreenModalComponentInput';
import { ActivatedRoute } from '../../../../../node_modules/@angular/router';
import { BecomeAdvisorComponent } from '../../advisor/become-advisor/become-advisor.component';
import { ConfirmEmailComponent } from '../../account/confirm-email/confirm-email.component';
import { ForgotPasswordResetComponent } from '../../account/forgot-password-reset/forgot-password-reset.component';
import { RegisterComponent } from '../../account/register/register.component';

@Component({
  selector: 'home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {

  constructor(public dialog: MatDialog, private route: ActivatedRoute) { }

  ngOnInit() {
    if (!!this.route.snapshot.queryParams['login']) {
      this.onLogin();
    } else if (!!this.route.snapshot.queryParams['becomeadvisor']) {
      this.onBecomeAdvisor();
    } else if (!!this.route.snapshot.queryParams['confirmemail']) {
      this.onConfirmEmail();
    } else if (!!this.route.snapshot.queryParams['resetpassword']) {
      this.onResetPassword();
    } else if (!!this.route.snapshot.queryParams['register']) {
      this.onRegister();
    }
  }

  onConfirmEmail() {
    let modalData = new FullscreenModalComponentInput();
    modalData.component = ConfirmEmailComponent;
    modalData.hiddenClose = true;
    this.dialog.open(FullscreenModalComponent, { data: modalData }); 
  }

  onRegister() {
    let modalData = new FullscreenModalComponentInput();
    modalData.component = RegisterComponent;
    this.dialog.open(FullscreenModalComponent, { data: modalData }); 
  }

  onResetPassword() {
    let modalData = new FullscreenModalComponentInput();
    modalData.component = ForgotPasswordResetComponent;
    this.dialog.open(FullscreenModalComponent, { data: modalData }); 
  }

  onBecomeAdvisor() {
    let modalData = new FullscreenModalComponentInput();
    modalData.component = BecomeAdvisorComponent;
    this.dialog.open(FullscreenModalComponent, { data: modalData }); 
  }

  onLogin() {
    let modalData = new FullscreenModalComponentInput();
    modalData.component = LoginComponent;
    this.dialog.open(FullscreenModalComponent, { data: modalData }); 
  }
}
