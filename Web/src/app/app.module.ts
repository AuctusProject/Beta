import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { NgModule } from '@angular/core';
import { SimpleNotificationsModule } from 'angular2-notifications';
import { ChartModule,HIGHCHARTS_MODULES } from 'angular-highcharts';
import stock from 'highcharts/modules/stock.src';
import more from 'highcharts/highcharts-more.src';

export function highchartsModules() {
  // apply Highcharts Modules to this array
  return [stock, more];
}

import { AppComponent } from './app.component';

import { Web3Service } from './services/web3.service';
import { LocalStorageService } from './services/local-storage.service';
import { HttpService } from './services/http.service';
import { AssetHistoryChartComponent } from './components/asset/asset-history-chart/asset-history-chart.component';
import { RecommendationDistributionComponent } from './components/recommendation-distribution/recommendation-distribution.component';
import { AdvisorCardComponent } from './components/advisor/advisor-card/advisor-card.component';
import { AppRoutingModule } from './app-routing.module';
import { AdvisorService } from './services/advisor.service';
import { HttpModule } from '../../node_modules/@angular/http';
import { HttpClientModule } from '../../node_modules/@angular/common/http';
import { AssetService } from './services/asset.service';
import { MessageSignatureComponent } from './components/account/message-signature/message-signature.component';
import { AccountService } from './services/account.service';
import { LoginComponent } from './components/account/login/login.component';
// import { MatModule } from './mat.module';
import { FormsModule, ReactiveFormsModule } from '../../node_modules/@angular/forms';
import { AuthGuard } from './providers/authGuard';
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
    ConfirmAdviceDialogComponent
  ],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    HttpModule,
    HttpClientModule,
    AppRoutingModule,
    SimpleNotificationsModule.forRoot(),
    ChartModule,
    FormsModule,
    ReactiveFormsModule,
    InfiniteScrollModule,
    MatModule
  ],
  providers: [
    HttpService,
    LocalStorageService,
    Web3Service,
    AdvisorService,
    AssetService,
    AccountService,
    AuthGuard,
    AuthRedirect,
    {provide:HIGHCHARTS_MODULES, useFactory:highchartsModules} 
  ],
  entryComponents:[ConfirmAdviceDialogComponent],
  bootstrap: [AppComponent]
})
export class AppModule { }
