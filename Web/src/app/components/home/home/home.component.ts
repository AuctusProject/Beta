import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ModalService } from '../../../services/modal.service';
import { AccountService } from '../../../services/account.service';
import { NavigationService } from '../../../services/navigation.service';
import { AuthRedirect } from '../../../providers/authRedirect';

@Component({
  selector: 'home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {

  constructor(private route: ActivatedRoute, 
    private modalService: ModalService,
    private accountService: AccountService,
    private authRedirect : AuthRedirect,
    private navigationService: NavigationService) { }

  ngOnInit() {
    if (!!this.route.snapshot.queryParams['configuration']) {
      this.modalService.setConfiguration();
    } else if (!!this.route.snapshot.queryParams['confirmemail']) {
      this.modalService.setConfirmEmail();
    } else if (!!this.route.snapshot.queryParams['resetpassword']) {
      this.modalService.setResetPassword();
    } else if (!!this.route.snapshot.queryParams['register']) {
      this.modalService.setRegister();
    } else if (!!this.route.snapshot.queryParams['becomeadvisor']) {
      this.modalService.setBecomeAdvisor();
    } else if (!!this.route.snapshot.queryParams['login']) {
      this.modalService.setLogin();
    } else if(this.isLoggedIn()) {
      this.authRedirect.redirectToHome(this.accountService.getLoginData());
    }
  }

  isLoggedIn(){
    return this.accountService.isLoggedIn();
  }
}
