import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { AssetResponse } from '../../../../model/asset/assetResponse';
import { ModalService } from '../../../../services/modal.service';
import { AssetService} from '../../../../services/asset.service';
import { AccountService } from '../../../../services/account.service';

@Component({
  selector: 'summary-tab',
  templateUrl: './summary-tab.component.html',
  styleUrls: ['./summary-tab.component.css']
})
export class SummaryTabComponent implements OnInit {
  showNewAdviceButton: boolean = false;
  @Input() asset: AssetResponse;
  @Output() onAdviceCreated: EventEmitter<any> = new EventEmitter<any>();

  constructor(private accountService: AccountService,
    private modalService: ModalService,
    private assetService: AssetService) { }

  ngOnInit() {
    this.showNewAdviceButton = this.accountService.isLoggedIn() && this.accountService.getLoginData().isAdvisor;
  }

 
  
  onNewAdviceClick() {
    this.modalService.setNewAdvice(this.asset.assetId).afterClosed().subscribe( () => this.onAdviceCreated.emit());
  }

  getOperationDisclaimer() {
    let sentence = this.asset.totalAdvisors + " ";
    if(this.asset.totalAdvisors == 1) {
      sentence += "expert"
    } else {
      sentence += "experts"
    }
    return "Expert recommendation for " + this.asset.code + " (Based on: " + sentence + ", last 30 days)";
  }
}
