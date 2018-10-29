import { Component, OnInit, Input } from '@angular/core';
import { AssetResponse } from '../../../../model/asset/assetResponse';
import { ModalService } from '../../../../services/modal.service';
import { AccountService } from '../../../../services/account.service';

@Component({
  selector: 'summary-tab',
  templateUrl: './summary-tab.component.html',
  styleUrls: ['./summary-tab.component.css']
})
export class SummaryTabComponent implements OnInit {
  showNewAdviceButton: boolean = false;
  @Input() asset: AssetResponse;

  constructor(private accountService: AccountService,
    private modalService: ModalService) { }

  ngOnInit() {
    this.showNewAdviceButton = this.accountService.isLoggedIn() && this.accountService.getLoginData().isAdvisor;
  }

  onNewAdviceClick() {
    this.modalService.setNewAdvice(this.asset.assetId);
  }

  getOperationDisclaimer() {
    let sentence = this.asset.totalAdvisors + " ";
    if(this.asset.totalAdvisors == 1) {
      sentence += "expert"
    } else {
      sentence += "experts"
    }
    return "Expert signal for " + this.asset.code + " (Based on: " + sentence + ", last 30 days)";
  }
}
