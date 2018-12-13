import { Component, OnInit, Input, ViewChild, EventEmitter, Output } from '@angular/core';
import { AssetResponse } from '../../../model/asset/assetResponse';
import { SetTradeComponent } from './set-trade/set-trade.component';
import { AccountService } from '../../../services/account.service';

@Component({
  selector: 'new-trade',
  templateUrl: './new-trade.component.html',
  styleUrls: ['./new-trade.component.css']
})
export class NewTradeComponent implements OnInit {
  @Input() asset: AssetResponse;
  @ViewChild("Limit") Limit: SetTradeComponent;
  @ViewChild("Market") Market: SetTradeComponent;
  @Output() orderCreated = new EventEmitter<void>();
  logged: boolean = false;
  
  constructor(private accountService: AccountService) { }

  ngOnInit() {
    this.logged = this.accountService.isLoggedIn();
  }

  onOrderCreated() {
    this.orderCreated.emit();
    if (this.Limit) {
      this.Limit.clear();
    }
    if (this.Market) {
      this.Market.clear();
    }
  }
}
