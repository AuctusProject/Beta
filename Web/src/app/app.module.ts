import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { NgModule, APP_INITIALIZER } from '@angular/core';
import { SimpleNotificationsModule } from 'angular2-notifications';
import { ShareButtonModule } from '@ngx-share/button';
import { ChartsModule } from 'ng2-charts';

import { AppComponent } from './app.component';
import { FlexLayoutModule } from "@angular/flex-layout";
import { LocalStorageService } from './services/local-storage.service';
import { HttpService } from './services/http.service';
import { RecommendationDistributionComponent } from './components/recommendation-distribution/recommendation-distribution.component';
import { AppRoutingModule } from './app-routing.module';
import { AdvisorService } from './services/advisor.service';
import { HttpModule } from '@angular/http';
import { HttpClientModule } from '@angular/common/http';
import { AssetService } from './services/asset.service';
import { AccountService } from './services/account.service';
import { LoginComponent } from './components/account/login/login.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { AuthRedirect } from './providers/authRedirect';
import { ConfirmEmailComponent } from './components/account/confirm-email/confirm-email.component';
import { BecomeAdvisorComponent } from './components/advisor/become-advisor/become-advisor.component';
import { ForgotPasswordComponent } from './components/account/forgot-password/forgot-password.component';
import { ForgotPasswordResetComponent } from './components/account/forgot-password-reset/forgot-password-reset.component';
import { ChangePasswordComponent } from './components/account/change-password/change-password.component';
import { ListAssetsComponent } from './components/asset/list-assets/list-assets.component';
import { AssetDetailsComponent } from './components/asset/asset-details/asset-details.component';
import { InfiniteScrollModule } from 'ngx-infinite-scroll';
import { MatModule } from './mat.module';
import { ConfigService, CONFIG } from './services/config.service';
import { ReferralDetailsComponent } from './components/account/referral-details/referral-details.component';
import { NavigationService } from './services/navigation.service';
import { GlobalSearchComponent } from './components/util/global-search/global-search.component';
import { NgHttpLoaderModule } from 'ng-http-loader'; 
import { TopLoadingComponent } from './components/util/top-loading/top-loading.component';
import { Angular2PromiseButtonModule } from 'angular2-promise-buttons';
import { AdvisorEditComponent } from './components/advisor/advisor-edit/advisor-edit.component';
import { HomeComponent } from './components/home/home/home.component';
import { TopExpertsComponent } from './components/advisor/top-experts/top-experts.component';
import { FileUploaderComponent } from './components/util/file-uploader/file-uploader.component';
import { BarRatingModule } from "ngx-bar-rating";
import { RecaptchaComponent }  from './components/util/recaptcha/recaptcha.component';
import { RecaptchaModule } from 'ng-recaptcha';
import { RecaptchaFormsModule } from 'ng-recaptcha/forms';
import { SocialLoginModule, AuthServiceConfig, GoogleLoginProvider, FacebookLoginProvider } from "angular5-social-login";
import { HeaderComponent } from './components/util/header/header.component';
import { FullscreenModalComponent } from './components/util/fullscreen-modal/fullscreen-modal.component';
import { ModalDirective } from './directives/modal.directive';
import { MAT_DIALOG_DEFAULT_OPTIONS, MatPaginatorModule, MatTableModule } from '@angular/material';
import { MessageFullscreenModalComponent } from './components/util/message-fullscreen-modal/message-fullscreen-modal.component';
import { PercentageDisplayComponent } from './components/util/percentage-display/percentage-display.component';
import { IconsModule } from './icons.module';
import { InheritanceInputComponent } from './components/util/inheritance-input/inheritance-input.component';
import { NewsletterComponent } from './components/util/newsletter/newsletter.component';
import { CoinSearchComponent } from './components/util/coin-search/coin-search.component';
import { GeneralRecommendationTagComponent } from './components/util/general-recommendation-tag/general-recommendation-tag.component';
import { EntryOptionComponent } from './components/account/entry-option/entry-option.component';
import { ModalService } from './services/modal.service';
import { LocalCacheService } from './services/local-cache.service';
import { ValueDisplayPipe } from './util/value-display.pipe';
import { NewsService } from './services/news.service';
import { TickerService } from './services/ticker.service';
import { HighlightFieldComponent } from './components/util/highlight-field/highlight-field.component';
import { TickerFieldComponent } from './components/util/ticker-field/ticker-field.component';
import { TickerPercentageFieldComponent } from './components/util/ticker-percentage-field/ticker-percentage-field.component';
import { TradeService } from './services/trade.service';
import { PortfolioComponent } from './components/trade/portfolio/portfolio.component';
import { HistoryComponent } from './components/trade/portfolio/history/history.component';
import { OpenPositionsComponent } from './components/trade/portfolio/open-positions/open-positions.component';
import { OrdersComponent } from './components/trade/portfolio/orders/orders.component';
import { BalanceInfoComponent } from './components/account/balance-info/balance-info.component';
import { LeftMenuComponent } from './components/util/left-menu/left-menu.component';
import { UserInfoComponent } from './components/account/user-info/user-info.component';
import { EventsServiceModule } from 'angular-event-service/dist';
import { ExpertPictureComponent } from './components/advisor/expert-picture/expert-picture.component';
import { FollowUnfollowComponent } from './components/util/follow-unfollow/follow-unfollow.component';
import { ExpertsTableComponent } from './components/advisor/experts-table/experts-table.component';
import { WatchlistComponent } from './components/account/watchlist/watchlist.component';
import { AssetsTableComponent } from './components/asset/assets-table/assets-table.component';
import { TradingContestComponent } from './components/advisor/trading-contest/trading-contest.component';
import { OrdersTableComponent } from './components/trade/portfolio/open-positions/orders-table/orders-table.component';
import { PrizeBoxComponent } from './components/advisor/trading-contest/prize-box/prize-box.component';
import { TickerProfitFieldComponent } from './components/util/ticker-profit-field/ticker-profit-field.component';
import { TradingViewChartComponent } from './components/util/trading-view-chart/trading-view-chart.component';
import { NewTradeComponent } from './components/trade/new-trade/new-trade.component';
import { SetTradeComponent } from './components/trade/new-trade/set-trade/set-trade.component';
import { HallOfFameComponent } from './components/advisor/trading-contest/hall-of-fame/hall-of-fame.component';
import { ExpertMiniCardComponent } from './components/advisor/expert-mini-card/expert-mini-card.component';
import { MomentModule } from 'ngx-moment';
import { AssetSummaryComponent } from './components/asset/asset-summary/asset-summary.component';
import { OrderPositionTabComponent } from './components/trade/order-position-tab/order-position-tab.component';
import { ConfirmationDialogComponent } from './components/util/confirmation-dialog/confirmation-dialog.component';
import { EditOrderComponent } from './components/trade/edit-order/edit-order.component';
import { PerformanceComponent } from './components/trade/portfolio/performance/performance.component';
import { PerformanceClosedPositionsComponent } from './components/trade/portfolio/performance/perfomance-closed-positions/perfomance-closed-positions.component';
import { TradeSummaryComponent } from './components/trade/portfolio/performance/trade-summary/trade-summary.component';
import { EditTradeValueComponent } from './components/trade/edit-trade-value/edit-trade-value.component';
import { MenuItemComponent } from './components/util/left-menu/menu-item/menu-item.component';
import { HomeHeaderComponent } from './components/home/home-header/home-header.component';
import { TopImageComponent } from './components/home/top-image/top-image.component';
import { CountdownComponent } from './components/util/countdown/countdown.component';
import { PercentageDisplayPipe } from './util/percentage-display.pipe';
import { DailyPerformanceChartComponent } from './components/trade/portfolio/performance/daily-performance-chart/daily-performance-chart.component';
import { NewTradeWindowComponent } from './components/trade/new-trade/new-trade-window/new-trade-window.component';
import { PerfomanceOpenPositionsComponent } from './components/trade/portfolio/performance/perfomance-open-positions/perfomance-open-positions.component';
import { MiniTradingViewChartComponent } from './components/util/mini-trading-view-chart/mini-trading-view-chart.component';
import { MiniViewComponent } from './components/util/mini-view/mini-view.component';
import { AssetMiniViewComponent } from './components/asset/asset-mini-view/asset-mini-view.component';
import { AssetMiniViewTableComponent } from './components/asset/asset-mini-view/asset-mini-view-table/asset-mini-view-table.component';
import { PortfolioMiniViewComponent } from './components/trade/portfolio/portfolio-mini-view/portfolio-mini-view.component';
import { TrendingAssetsComponent } from './components/asset/trending-assets/trending-assets.component';
import { NewsListComponent } from './components/util/mini-view/news-list/news-list.component';

export function loadConfigService(configService: ConfigService): Function
{
  return () => { return configService.load() }; 
}

export function getAuthServiceConfigs() {
  let config = new AuthServiceConfig(
      [
        {
          id: FacebookLoginProvider.PROVIDER_ID,
          provider: new FacebookLoginProvider("2044979652220211")
        },
        {
          id: GoogleLoginProvider.PROVIDER_ID,
          provider: new GoogleLoginProvider("367680444115-ceggf3epb575veih8mb0rm0jdi37qede.apps.googleusercontent.com")
        }
      ]
  );
  return config;
}


@NgModule({
  declarations: [
    AppComponent,
    RecommendationDistributionComponent,
    LoginComponent,
    ConfirmEmailComponent,
    BecomeAdvisorComponent,
    ForgotPasswordComponent,
    ForgotPasswordResetComponent,
    ChangePasswordComponent,
    ListAssetsComponent,
    AssetDetailsComponent,
    ReferralDetailsComponent,
    GlobalSearchComponent,
    TopLoadingComponent,
    AdvisorEditComponent,
    HomeComponent,
    TopExpertsComponent,
    FileUploaderComponent,
    RecaptchaComponent,
    HeaderComponent,
    FullscreenModalComponent,
    ModalDirective,
    MessageFullscreenModalComponent,
    PercentageDisplayComponent,
    InheritanceInputComponent,
    NewsletterComponent,
    CoinSearchComponent,
    EntryOptionComponent,
    GeneralRecommendationTagComponent,
    ValueDisplayPipe,
    HighlightFieldComponent,
    TickerFieldComponent,
    TickerPercentageFieldComponent,
    PortfolioComponent,
    HistoryComponent,
    OpenPositionsComponent,
    OrdersComponent,
    BalanceInfoComponent,
    LeftMenuComponent,
    UserInfoComponent,
    ExpertPictureComponent,
    FollowUnfollowComponent,
    ExpertsTableComponent,
    WatchlistComponent,
    AssetsTableComponent,
    OrdersTableComponent,
    TradingContestComponent,
    PrizeBoxComponent,
    TickerProfitFieldComponent,
    TradingViewChartComponent,
    MiniTradingViewChartComponent,
    NewTradeComponent,
    SetTradeComponent,
    HallOfFameComponent,
    ExpertMiniCardComponent,
    AssetSummaryComponent,
    OrderPositionTabComponent,
    ConfirmationDialogComponent,
    PerformanceComponent,
    PerformanceClosedPositionsComponent,
    TradeSummaryComponent,
    EditOrderComponent,
    EditTradeValueComponent,
    MenuItemComponent,
    HomeHeaderComponent,
    TopImageComponent,
    PercentageDisplayPipe,
    CountdownComponent,
    DailyPerformanceChartComponent,
    NewTradeWindowComponent,
    PerfomanceOpenPositionsComponent,
    AssetMiniViewComponent,
    AssetMiniViewTableComponent,
    MiniViewComponent,
    PortfolioMiniViewComponent,
    TrendingAssetsComponent,
    NewsListComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'Web' }),
    BrowserAnimationsModule,
    HttpModule,
    HttpClientModule,
    NgHttpLoaderModule,
    AppRoutingModule,
    SimpleNotificationsModule.forRoot(),
    Angular2PromiseButtonModule
      .forRoot({
        spinnerTpl: '<span class="btn-spinner"></span>',
        disableBtn: true,
        btnLoadingClass: 'is-loading',
        handleCurrentBtnOnly: true,
    }),
    FormsModule,
    ReactiveFormsModule,
    InfiniteScrollModule,
    MatModule,
    IconsModule,
    FlexLayoutModule,
    BarRatingModule,
    SocialLoginModule,
    RecaptchaModule.forRoot(), 
    RecaptchaFormsModule,
    MomentModule,
    ShareButtonModule.forRoot(),
    EventsServiceModule.forRoot(),
    MatTableModule,
    MatPaginatorModule,
    ChartsModule
  ],
  providers: [
    HttpService,
    LocalStorageService,
    LocalCacheService,
    AdvisorService,
    AssetService,
    AccountService,
    TradeService,
    NewsService,
    NavigationService,
    AuthRedirect,
    ConfigService,
    ModalService,
    TickerService,
    { provide: APP_INITIALIZER, useFactory: loadConfigService , deps: [ConfigService], multi: true },
    { provide: AuthServiceConfig, useFactory: getAuthServiceConfigs },
    { provide: MAT_DIALOG_DEFAULT_OPTIONS, useValue: { width: '95vw', height: '92%', hasBackdrop: true, disableClose: false, panelClass: 'fullscreen-modal'}}
  ],
  entryComponents:[
    FullscreenModalComponent, 
    TopLoadingComponent, 
    LoginComponent, 
    ForgotPasswordComponent, 
    MessageFullscreenModalComponent,
    BecomeAdvisorComponent,
    ConfirmEmailComponent,
    AdvisorEditComponent,
    ForgotPasswordResetComponent,
    ChangePasswordComponent,
    ReferralDetailsComponent,
    EntryOptionComponent,
    ConfirmationDialogComponent,
    EditOrderComponent,
    EditTradeValueComponent,
    NewTradeWindowComponent
  ],
  exports: [TopLoadingComponent],
  bootstrap: [AppComponent]
})
export class AppModule { }
