import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '../../../../../node_modules/@angular/router';
import { ModalService } from '../../../services/modal.service';

@Component({
  selector: 'home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {

  constructor(private route: ActivatedRoute, private modalService: ModalService) { }

  ngOnInit() {
    if (!!this.route.snapshot.queryParams['login']) {
      this.modalService.setLogin();
    } else if (!!this.route.snapshot.queryParams['becomeadvisor']) {
      this.modalService.setBecomeAdvisor();
    } else if (!!this.route.snapshot.queryParams['confirmemail']) {
      this.modalService.setConfirmEmail();
    } else if (!!this.route.snapshot.queryParams['resetpassword']) {
      this.modalService.setResetPassword();
    } else if (!!this.route.snapshot.queryParams['register']) {
      this.modalService.setRegister();
    } else if (!!this.route.snapshot.queryParams['configuration']) {
      this.modalService.setConfiguration();
    }
  }
}
