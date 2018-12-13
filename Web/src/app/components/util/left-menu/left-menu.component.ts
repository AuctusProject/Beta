import { Component, OnInit, Input } from '@angular/core';
import { LoginResponse } from '../../../model/account/loginResponse';
import { AccountService } from '../../../services/account.service';
import { AdvisorResponse } from '../../../model/advisor/advisorResponse';

@Component({
  selector: 'left-menu',
  templateUrl: './left-menu.component.html',
  styleUrls: ['./left-menu.component.css']
})
export class LeftMenuComponent implements OnInit {
  loginData: LoginResponse;
  @Input() advisor: AdvisorResponse;
  
  constructor(private accountService : AccountService) { }

  ngOnInit() {
  }

  isLogged(): boolean {
    return this.accountService.isLoggedIn() && this.advisor != null;
  }

  logout(){
    this.accountService.logout();
    this.advisor = null;
  }
}
