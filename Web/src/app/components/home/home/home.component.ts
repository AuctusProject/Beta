import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ModalService } from '../../../services/modal.service';
import { AccountService } from '../../../services/account.service';
import { NavigationService } from '../../../services/navigation.service';
import { AuthRedirect } from '../../../providers/authRedirect';
import { CONFIG } from '../../../services/config.service';

@Component({
  selector: 'home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {

  constructor(private route: ActivatedRoute, 
    private modalService: ModalService,
    private accountService: AccountService,
    private navigationService: NavigationService) { }

  ngOnInit() {
    if (!!this.route.snapshot.queryParams['confirmemail']) {
      this.modalService.setConfirmEmail();
    } else if (!!this.route.snapshot.queryParams['resetpassword']) {
      this.modalService.setResetPassword();
    } else if (!!this.route.snapshot.queryParams['register']) {
      this.modalService.setRegister();
    } else if (!!this.route.snapshot.queryParams['becomeadvisor']) {
      this.modalService.setBecomeAdvisor();
    } else if (!!this.route.snapshot.queryParams['completeregistration']) {
      this.modalService.setCompleteRegistration();
    } else if (!!this.route.snapshot.queryParams['login']) {
      this.modalService.setLogin();
    } else if(this.isLoggedIn()) {
      this.navigationService.goToPortfolio();
    }
  }
  
  signup(){
    this.modalService.setRegister();
  }

  isLoggedIn() {
    return this.accountService.isLoggedIn();
  }

  getImageUrl(name: string) : string {
    return CONFIG.platformImgUrl.replace("{id}", name);
  }
}
