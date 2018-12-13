import { Component, OnInit, Inject, PLATFORM_ID, OnDestroy, ChangeDetectorRef, ViewChild } from '@angular/core';
import { TopLoadingComponent } from './components/util/top-loading/top-loading.component';
import { Router, NavigationStart, NavigationEnd } from '@angular/router';
import { NavigationService } from './services/navigation.service';
import { ModalService } from './services/modal.service';
import { AccountService } from './services/account.service';
import { CONFIG } from './services/config.service';
import { isPlatformBrowser } from '@angular/common';
import { TickerService } from './services/ticker.service';
import { MediaMatcher } from '@angular/cdk/layout';
import { AdvisorDataService } from './services/advisor-data.service';
import { AdvisorResponse } from './model/advisor/advisorResponse';
import { EventsService } from 'angular-event-service/dist';
import { Subscription } from 'rxjs';
import { HubConnectionBuilder, HubConnection } from '@aspnet/signalr';
import { OrderResponse } from './model/trade/orderResponse';
import { Constants } from './util/constants';
import { NotificationsService } from 'angular2-notifications';
import { ValueDisplayPipe } from './util/value-display.pipe';
import { MatSidenav } from '@angular/material';
import 'rxjs/add/operator/filter';
import { LoginResponse } from './model/account/loginResponse';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
  providers: [AdvisorDataService]
})
export class AppComponent implements OnInit, OnDestroy {
  public topLoading = TopLoadingComponent.prototype.constructor;
  advisor: AdvisorResponse;
  advisorResponseSubscription:Subscription = null;
  connection: HubConnection;
  hubStarted: boolean = false;

  mobileQuery: MediaQueryList;
  miniViewQuery: MediaQueryList;
  @ViewChild("snav") sidenav: MatSidenav;
  private _mobileQueryListener: () => void;
  private _miniViewQueryListener: () => void;

  constructor(private router: Router, 
    private navigationService : NavigationService,
    private modalService: ModalService,
    private accountService: AccountService,
    private tickerService: TickerService,
    private advisorDataService: AdvisorDataService,
    private eventsService: EventsService,
    private notificationsService: NotificationsService,
    changeDetectorRef: ChangeDetectorRef, 
    media: MediaMatcher,
    @Inject(PLATFORM_ID) private platformId: Object
    ) {
      if(this.isBrowser()){ 
        this.tickerService.initialize();
      }
      this.mobileQuery = media.matchMedia('(max-width: 959px)');
      this._mobileQueryListener = () => changeDetectorRef.detectChanges();
      this.mobileQuery.addListener(this._mobileQueryListener); 
      this.miniViewQuery = media.matchMedia('(min-width: 1366px)');
      this._miniViewQueryListener = () => changeDetectorRef.detectChanges();
      this.miniViewQuery.addListener(this._miniViewQueryListener); 
      router.events
      .filter(event => event instanceof NavigationStart)
      .subscribe((event:NavigationStart) => {
        if(this.mobileQuery.matches && this.sidenav){
          this.sidenav.close();
        }
      });
    }

  ngOnInit(){
    if(this.isLogged()){
      this.validateLoginAndStartLoggedActions();
    }
    this.eventsService.on("onLogin", () => {
      this.afterLogin();
    });
    this.eventsService.on("onUpdateAdvisor", () => this.onLogin());
  }

  validateLoginAndStartLoggedActions(){
    this.accountService.getUserData().subscribe(ret =>
    {
      this.accountService.setLoginData(ret);
      this.afterLogin();
    });
  }

  afterLogin(){
    this.onLogin();
    this.initializeHubConnection();
  }

  initializeHubConnection() {
    if(!this.hubStarted && isPlatformBrowser(this.platformId)) {
      this.connection = new HubConnectionBuilder().withUrl(CONFIG.apiUrl + "auctusHub", { accessTokenFactory: () => this.accountService.getAccessToken() }).build();
      this.connection.serverTimeoutInMilliseconds = 40000;
      this.connection.onclose(() => this.startHubConnection(this));
      this.connection.on("onReachStopLoss", (data) => this.onHubDataReceive(Constants.OrderActionType.StopLoss, data, this));
      this.connection.on("onReachTakeProfit", (data) => this.onHubDataReceive(Constants.OrderActionType.TakeProfit, data, this));
      this.connection.on("onReachOrderLimit", (data) => this.onHubDataReceive(Constants.OrderActionType.Limit, data, this));
      this.startHubConnection(this);
    }
  }

  startHubConnection(_this){
    this.connection
    .start()
    .then(() => {
      _this.hubStarted = true;
      console.log('Connection started!');
    })
    .catch(err => {
      console.log('Error while establishing connection :('); 
      setTimeout(() => _this.zone.run(() => _this.startConnection(_this), 5000));
    });
  }

  onHubDataReceive(actionType: number, data: OrderResponse[], _this) {
    if (data && data.length > 0) {
      this.eventsService.broadcast("onUpdateAdvisor", data);
      for (let i = 0; i < data.length; ++i) {
        let message = " for <b>" + new ValueDisplayPipe().transform(data[i].quantity, '') + " " + data[i].assetCode + "</b> was executed at <i>" + new ValueDisplayPipe().transform(data[i].price) + "</i>.";
        if (actionType == Constants.OrderActionType.Limit) {
          message = "An order limit" + message;
          this.notificationsService.warn(null, message, this.orderNotificationOptions);
        } else if (actionType == Constants.OrderActionType.StopLoss) {
          message = "A stop loss order" + message;
          this.notificationsService.error(null, message, this.orderNotificationOptions);
        } else if (actionType == Constants.OrderActionType.TakeProfit) {
          message = "A take profit order" + message;
          this.notificationsService.success(null, message, this.orderNotificationOptions);
        }
      }
    }
  }

  public orderNotificationOptions = {
    timeOut: 30000,
    icons: {success: ' ', alert: ' ', warn: ' ', error: ' '}
  };
  
  public onLogout(){
    if (this.advisorResponseSubscription) {
      this.advisorResponseSubscription.unsubscribe();
      this.advisorResponseSubscription = null;
    }
    this.advisorDataService.destroy();
  }

  public onLogin(){
    if(this.accountService.getLoginData().isAdvisor){
      if (!this.advisorResponseSubscription) {
        this.advisorDataService.initialize();    
        this.advisorResponseSubscription = this.advisorDataService.advisorResponse().subscribe(ret => {
          this.advisor = ret;
        });
      } else {
        this.advisorDataService.refresh();
      }
    }
  }
  
  public isBrowser(){
    return isPlatformBrowser(this.platformId);
  }

  public notificationOptions = {
    position: ["bottom", "left"],
    maxStack: 6,
    preventDuplicates: false,
    preventLastDuplicates: false,
    clickToClose: true,
    timeOut: 5000,
    pauseOnHover: true
  };

  public showEmptyPage() : boolean {
    return this.navigationService.isSameRoute("", this.router.url);
  }

  isLogged(): boolean {
    return this.accountService.isLoggedIn();
  }

  ngOnDestroy(): void {
    this.mobileQuery.removeListener(this._mobileQueryListener);
    if (this.advisorResponseSubscription) {
      this.advisorResponseSubscription.unsubscribe();
      this.advisorResponseSubscription = null;
    }
    this.advisorDataService.destroy();
  }
}
