import { Component, OnInit, Input, ViewChild, OnDestroy, SimpleChanges } from '@angular/core';
import { AssetResponse } from '../../../model/asset/assetResponse';
import { OrdersComponent } from '../portfolio/orders/orders.component';
import { OpenPositionsComponent } from '../portfolio/open-positions/open-positions.component';
import { AdvisorDataService } from '../../../services/advisor-data.service';
import { LoginResponse } from '../../../model/account/loginResponse';
import { AccountService } from '../../../services/account.service';
import { Subscription } from 'rxjs';
import { AssetPositionResponse, PositionResponse } from '../../../model/advisor/advisorResponse';
import { EventsService } from 'angular-event-service/dist';
import { MatTabGroup } from '@angular/material';
import { HistoryComponent } from '../portfolio/history/history.component';

@Component({
  selector: 'order-position-tab',
  templateUrl: './order-position-tab.component.html',
  styleUrls: ['./order-position-tab.component.css'],
  providers: [AdvisorDataService]
})
export class OrderPositionTabComponent implements OnInit, OnDestroy {
  @Input() asset: AssetResponse;
  @ViewChild("Position") Position: OpenPositionsComponent;
  @ViewChild("Order") Order: OrdersComponent;
  @ViewChild("History") History: HistoryComponent;
  @ViewChild("OrdersTabs") OrdersTabs: MatTabGroup;

  loginData: LoginResponse;
  openPositionResponseSubscription: Subscription;
  closedPositionSubscription: Subscription; 
  initialized: boolean = false;

  constructor(private advisorDataService: AdvisorDataService,
    private accountService: AccountService,
    private eventsService: EventsService) { }

  ngOnInit() {
    this.loginData = this.accountService.getLoginData();
    this.initialize();
    if (!!this.loginData) {
      this.eventsService.on("onUpdateAdvisor", () => this.refresh());
    }
  }

  initialize() {
    if (!!this.loginData) {
      this.advisorDataService.initialize(this.loginData.id);
      this.openPositionResponseSubscription = this.advisorDataService.openPosition(this.asset.assetId).subscribe(
        ret => {
          this.Position.setDataSource(ret ? [ret] : []);
        }
      );
      this.closedPositionSubscription = this.advisorDataService.closePosition(this.asset.assetId).subscribe(
        ret => {
          let response = new PositionResponse();
          response.averagePrice = ret.positionResponse.averagePrice;
          response.totalInvested = ret.positionResponse.totalInvested;
          response.totalProfit = ret.positionResponse.totalProfit;
          response.totalVirtual = ret.positionResponse.totalVirtual;
          response.totalQuantity = ret.positionResponse.totalQuantity;
          response.averageReturn = ret.positionResponse.averageReturn;
          response.successRate = ret.positionResponse.successRate;
          response.orderCount = ret.positionResponse.orderCount;
          response.successCount = ret.positionResponse.successCount;
          response.totalFee = ret.positionResponse.totalFee;
          response.averageTradeMinutes = ret.positionResponse.averageTradeMinutes;
          response.type = ret.positionResponse.type;
          this.History.setPosition(response);
        });
      this.initialized = true;
    }
  }

  ngOnChanges(changes: SimpleChanges) {
    if (this.initialized && !!changes.asset && changes.asset.previousValue.assetId != changes.asset.currentValue.assetId) {
      this.ngOnDestroy();
      this.asset = changes.asset.currentValue;
      this.initialize();
    }
  }

  refresh() {
    if (!!this.loginData) {
      this.advisorDataService.refresh();
    }
  }

  ngOnDestroy(){
    if (this.openPositionResponseSubscription) this.openPositionResponseSubscription.unsubscribe();
    if (this.closedPositionSubscription) this.closedPositionSubscription.unsubscribe();
    if (this.advisorDataService) this.advisorDataService.destroy();
    this.initialized = false;
  }

  closeAllOpenPositions(assetId: number, assetCode: string) {
    if (this.Position) {
      this.Position.closeAllOpenPositions(assetId, assetCode);
    }
   }

  cancelAllOpenOrders() {
    if (this.Order) {
      this.Order.cancelAllOpenOrders();
    }
  }

  showCancelOpenOrders() {
    return this.OrdersTabs.selectedIndex == 1 && this.Order.hasOpenOrders();
  }

  showCloseOpenPositions() {
    return this.OrdersTabs.selectedIndex == 0 && this.Position.hasOpenPositions();
  }
}
