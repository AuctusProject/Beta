import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { NgModule, APP_INITIALIZER } from '@angular/core';
import { SimpleNotificationsModule } from 'angular2-notifications';
import { ChartModule,HIGHCHARTS_MODULES } from 'angular-highcharts';
import stock from 'highcharts/modules/stock.src';
import more from 'highcharts/highcharts-more.src';

export function highchartsModules() {
  // apply Highcharts Modules to this array
  return [stock, more];
}

import { AppComponent } from './app.component';
import { FlexLayoutModule } from "@angular/flex-layout";
import { Web3Service } from './services/web3.service';
import { LocalStorageService } from './services/local-storage.service';
import { HttpService } from './services/http.service';
import { AssetHistoryChartComponent } from './components/asset/asset-history-chart/asset-history-chart.component';
import { RecommendationDistributionComponent } from './components/recommendation-distribution/recommendation-distribution.component';
import { AdvisorCardComponent } from './components/advisor/advisor-card/advisor-card.component';
import { AppRoutingModule } from './app-routing.module';
import { AdvisorService } from './services/advisor.service';
import { HttpModule } from '@angular/http';
import { HttpClientModule } from '@angular/common/http';
import { AssetService } from './services/asset.service';
import { MessageSignatureComponent } from './components/account/message-signature/message-signature.component';
import { AccountService } from './services/account.service';
import { LoginComponent } from './components/account/login/login.component';
// import { MatModule } from './mat.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { AuthRedirect } from './providers/authRedirect';
import { ConfirmEmailComponent } from './components/account/confirm-email/confirm-email.component';
import { RegisterComponent } from './components/account/register/register.component';
import { BecomeAdvisorComponent } from './components/advisor/become-advisor/become-advisor.component';
import { ForgotPasswordComponent } from './components/account/forgot-password/forgot-password.component';
import { ForgotPasswordResetComponent } from './components/account/forgot-password-reset/forgot-password-reset.component';
import { ChangePasswordComponent } from './components/account/change-password/change-password.component';
import { ListAdvisorsComponent } from './components/advisor/list-advisors/list-advisors.component';
import { AdvisorDetailsComponent } from './components/advisor/advisor-details/advisor-details.component';
import { ListAssetsComponent } from './components/asset/list-assets/list-assets.component';
import { AssetCardComponent } from './components/asset/asset-card/asset-card.component';
import { AssetDetailsComponent } from './components/asset/asset-details/asset-details.component';
import { AdvicesComponent } from './components/advisor/advices/advices.component';
import { InfiniteScrollModule } from 'ngx-infinite-scroll';
import { NewAdviceComponent } from './components/advisor/new-advice/new-advice.component';
import { MatModule } from './mat.module';
import { ConfirmAdviceDialogComponent } from './components/advisor/new-advice/confirm-advice-dialog/confirm-advice-dialog.component';
import { ConfigService } from './services/config.service';
import { ReferralComponent } from './components/account/referral/referral.component';
import { ConfigurationComponent } from './components/account/configuration/configuration.component';
import { NavigationService } from './services/navigation.service';
import { DashboardComponent } from './components/admin/dashboard/dashboard.component';
import { GlobalSearchComponent } from './components/util/global-search/global-search.component';
import { AdvisorsRequestsComponent } from './components/admin/advisors-requests/advisors-requests.component';
import { NgHttpLoaderModule } from 'ng-http-loader'; 
import { TopLoadingComponent } from './components/util/top-loading/top-loading.component';
import {Angular2PromiseButtonModule} from 'angular2-promise-buttons/dist';
import { AdvisorEditComponent } from './components/advisor/advisor-edit/advisor-edit.component';
import { RecommendationBoxComponent } from './components/home/recommendation-box/recommendation-box.component';
import { HomeComponent } from './components/home/home/home.component';
import { TopAdvisorsComponent } from './components/advisor/top-advisors/top-advisors.component';
import { FileUploaderComponent } from './components/util/file-uploader/file-uploader.component';
import { BarRatingModule } from "ngx-bar-rating";
import { RecaptchaComponent }  from './components/util/recaptcha/recaptcha.component';
import { RecaptchaModule } from 'ng-recaptcha';
import { RecaptchaFormsModule } from 'ng-recaptcha/forms';
import { SocialLoginModule, AuthServiceConfig, GoogleLoginProvider, FacebookLoginProvider } from "angular5-social-login";
import { HeaderComponent } from './components/header/header.component';
import { FullscreenModalComponent } from './components/util/fullscreen-modal/fullscreen-modal.component';
import { ModalDirective } from './directives/modal.directive';
import { MAT_DIALOG_DEFAULT_OPTIONS } from '@angular/material';
import { MessageFullscreenModalComponent } from './components/util/message-fullscreen-modal/message-fullscreen-modal.component';
import { RecommendationBoxListComponent } from './components/home/recommendation-box-list/recommendation-box-list.component';

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
    AssetHistoryChartComponent,
    RecommendationDistributionComponent,
    AdvisorCardComponent,
    MessageSignatureComponent,
    LoginComponent,
    RegisterComponent,
    ConfirmEmailComponent,
    BecomeAdvisorComponent,
    ForgotPasswordComponent,
    ForgotPasswordResetComponent,
    ChangePasswordComponent,
    ListAdvisorsComponent,
    AdvisorDetailsComponent,
    ListAssetsComponent,
    AssetCardComponent,
    AssetDetailsComponent,
    AdvicesComponent,
    NewAdviceComponent,
    ConfirmAdviceDialogComponent,
    ReferralComponent,
    ConfigurationComponent, 
    DashboardComponent,
    GlobalSearchComponent,
    AdvisorsRequestsComponent,
    TopLoadingComponent,
    AdvisorEditComponent,
    RecommendationBoxComponent,
    HomeComponent,
    TopAdvisorsComponent,
    FileUploaderComponent,
    RecaptchaComponent,
    HeaderComponent,
    FullscreenModalComponent,
    ModalDirective,
    MessageFullscreenModalComponent,
    RecommendationBoxListComponent
  ],
  imports: [
    BrowserModule,
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
        handleCurrentBtnOnly: false,
    }),
    ChartModule,
    FormsModule,
    ReactiveFormsModule,
    InfiniteScrollModule,
    MatModule,
    FlexLayoutModule,
    BarRatingModule,
    SocialLoginModule,
    RecaptchaModule.forRoot(), 
    RecaptchaFormsModule
  ],
  providers: [
    HttpService,
    LocalStorageService,
    Web3Service,
    AdvisorService,
    AssetService,
    AccountService,
    NavigationService,
    AuthRedirect,
    { provide:HIGHCHARTS_MODULES, useFactory:highchartsModules },
    ConfigService,
    { provide: APP_INITIALIZER, useFactory: loadConfigService , deps: [ConfigService], multi: true },
    { provide: AuthServiceConfig, useFactory: getAuthServiceConfigs },
    { provide: MAT_DIALOG_DEFAULT_OPTIONS, useValue: { width: '100%', height: '92%', hasBackdrop: true, maxWidth: '95vw', disableClose: false, panelClass: 'fullscreen-modal'}}
  ],
  entryComponents:[
    FullscreenModalComponent, 
    ConfirmAdviceDialogComponent, 
    TopLoadingComponent, 
    LoginComponent, 
    ForgotPasswordComponent, 
    MessageFullscreenModalComponent,
    BecomeAdvisorComponent
  ],
  exports: [TopLoadingComponent],
  bootstrap: [AppComponent]
})
export class AppModule { }
