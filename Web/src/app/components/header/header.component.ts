import { Component, OnInit } from '@angular/core';
import { AccountService } from '../../services/account.service';
import { LoginResponse } from '../../model/account/loginResponse';

@Component({
  selector: 'header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.css']
})
export class HeaderComponent implements OnInit {
  loginData : LoginResponse;
  constructor(private accountService : AccountService) { }

  ngOnInit() {
    this.loginData = this.accountService.getLoginData();
  }

  logout(){
    this.accountService.logout();
    this.loginData = null;
  }
}
