import { Component, OnInit, OnDestroy, ViewChild, Input, ChangeDetectorRef } from '@angular/core';
import { LoginResponse } from '../../../model/account/loginResponse';
import { AccountService } from '../../../services/account.service';
import { AdvisorDataService } from '../../../services/advisor-data.service';
import { Subscription } from 'rxjs';
import { AdvisorResponse, PositionResponse, AssetPositionResponse } from '../../../model/advisor/advisorResponse';
import { EventsService } from 'angular-event-service/dist';
import { OpenPositionsComponent } from './open-positions/open-positions.component';
import { HistoryComponent } from './history/history.component';
import { OrdersComponent } from './orders/orders.component';
import { ActivatedRoute } from '@angular/router';
import { Title, Meta } from '@angular/platform-browser';
import { PerformanceComponent } from './performance/performance.component';
import { MatTabGroup, MatTabChangeEvent } from '@angular/material';
@Component({
  selector: 'portfolio',
  templateUrl: './portfolio.component.html',
  styleUrls: ['./portfolio.component.css'],
  providers: [AdvisorDataService]
})
export class PortfolioComponent implements OnInit, OnDestroy {
  userId: number;
  isOwner: boolean;
  advisorResponseSubscription: Subscription;
  allOpenPositionResponseSubscription: Subscription;
  openPositionResponseSubscription: Subscription;
  allClosedPositionResponseSubscription: Subscription;
  closedPositionResponseSubscription: Subscription;
  advisor: AdvisorResponse;
  closedPosition: PositionResponse;
  openPosition: PositionResponse;
  metadataSet: boolean = false;
  showCancelOrdersButton: boolean = false;
  @Input() loadedUserId:number;

  @ViewChild("Position") Position: OpenPositionsComponent;
  @ViewChild("History") History: HistoryComponent;
  @ViewChild("Orders") Orders: OrdersComponent;
  @ViewChild("Performance") Performance: PerformanceComponent;
  @ViewChild("PortfolioTabs") PortfolioTabs: MatTabGroup;

  constructor(private route: ActivatedRoute, 
    private titleService: Title, 
    private metaTagService: Meta,
    private accountService: AccountService, 
    private advisorDataService: AdvisorDataService,
    private eventsService: EventsService,
    private changeDetectorRef: ChangeDetectorRef ) { }

  ngOnInit() {
    let self = this;
    let loginData = self.accountService.getLoginData();
    this.route.params.subscribe(params => 
      {
        if (params['id'] ) {
          self.userId = parseInt(params['id']);
          self.isOwner = self.userId == (loginData ? loginData.id : -1);
        } else if (!!loginData) {
          self.userId = loginData.id;
          self.isOwner = true;
        }
        self.initialize();
      }
    );
  }

  initialize() {
    if (this.userId) {
      this.advisorDataService.initialize(this.userId);

      this.advisorResponseSubscription = this.advisorDataService.advisorResponse().subscribe(
        ret => {
          this.advisor = ret;

          if (!this.metadataSet) {
            this.titleService.setTitle("Auctus Trading - " + ret.name);
            this.metaTagService.updateTag({name: 'description', content: ret.name + " - " + ret.description});
            this.metadataSet = true;
          }
        });

      this.closedPositionResponseSubscription = this.advisorDataService.closePositions().subscribe(
        ret => {
          this.closedPosition = ret;
          this.History.setPosition(ret);
        });

      this.allOpenPositionResponseSubscription = this.advisorDataService.listAllOpenPositions().subscribe(
        ret => {
          this.Position.setDataSource(ret);
          this.Performance.setAllOpenPosition(ret);
        });

      this.openPositionResponseSubscription = this.advisorDataService.openPositions().subscribe(
        ret => {
          this.openPosition = ret;
        });

      this.allClosedPositionResponseSubscription = this.advisorDataService.listAllClosePositions().subscribe(
        ret => {
          this.Performance.setAllClosedPosition(ret);
          this.History.setAssetPositions(ret);
        });

      this.eventsService.on("onUpdateAdvisor", () => this.refreshAll());
    }
  }

  refreshAll() {
    this.advisorDataService.refresh();
    this.History.refresh();
    this.Position.refresh();
  }

  ngOnDestroy(){
    if (this.openPositionResponseSubscription) this.openPositionResponseSubscription.unsubscribe();
    if (this.allOpenPositionResponseSubscription) this.allOpenPositionResponseSubscription.unsubscribe();
    if (this.closedPositionResponseSubscription) this.closedPositionResponseSubscription.unsubscribe();
    if (this.allClosedPositionResponseSubscription) this.allClosedPositionResponseSubscription.unsubscribe();
    if (this.advisorResponseSubscription) this.advisorResponseSubscription.unsubscribe();
    this.advisorDataService.destroy();
  }

  getProfit24hPercentage(){
    if(!this.advisor.profit24hPercentage)
      return 0;
    return this.advisor.profit24hPercentage;
  }

  cancelAllOpenOrders() {
    this.Orders.cancelAllOpenOrders();
  }

  onTabChanged(event: MatTabChangeEvent) {
    this.showCancelOrdersButton = event.index == 2 && this.Orders.hasOpenOrders();
    this.changeDetectorRef.detectChanges();
  }
}
