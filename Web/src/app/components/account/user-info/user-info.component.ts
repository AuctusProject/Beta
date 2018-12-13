import { Component, OnInit, Input, OnDestroy, OnChanges } from '@angular/core';
import { AdvisorResponse } from '../../../model/advisor/advisorResponse';
import { CONFIG } from '../../../services/config.service';
import { ModalService } from '../../../services/modal.service';
import { AccountService } from '../../../services/account.service';
import { NavigationService } from '../../../services/navigation.service';

@Component({
  selector: 'user-info',
  templateUrl: './user-info.component.html',
  styleUrls: ['./user-info.component.css']
})
export class UserInfoComponent implements OnInit {
  @Input() advisor: AdvisorResponse;

  constructor(private modalService: ModalService, 
    private accountService: AccountService,
    private navigationService: NavigationService) { }

  ngOnInit(){
  }

  getAdvisorImgUrl(){
    return CONFIG.profileImgUrl.replace("{id}", this.advisor.urlGuid);
  }

  editAdvisor() {
    this.modalService.setEditAdvisor(this.advisor.userId);
  }

  isLogged(): boolean {
    return this.accountService.isLoggedIn();
  }

  logout() {
    this.accountService.logout();
    this.navigationService.goToHome();
  }
  
  login() {
    this.modalService.setLogin();
  }

  signUp() {
    this.modalService.setRegister();
  }

  getProfit24hPercentage(){
    if(!this.advisor.profit24hPercentage)
      return 0;
    return this.advisor.profit24hPercentage;
  }
}
