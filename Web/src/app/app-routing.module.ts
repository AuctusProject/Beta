import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthRedirect } from './providers/authRedirect';
import { MessageSignatureComponent } from './components/account/message-signature/message-signature.component';
import { ForgotPasswordResetComponent } from './components/account/forgot-password-reset/forgot-password-reset.component';
import { ChangePasswordComponent } from './components/account/change-password/change-password.component';
import { ExpertDetailsComponent } from './components/advisor/expert-details/expert-details.component';
import { ListAssetsComponent } from './components/asset/list-assets/list-assets.component';
import { AssetDetailsComponent } from './components/asset/asset-details/asset-details.component';
import { AdvicesComponent } from './components/advisor/advices/advices.component';
import { NewAdviceComponent } from './components/advisor/new-advice/new-advice.component';
import { ReferralComponent } from './components/account/referral/referral.component';
import { ConfigurationComponent } from './components/account/configuration/configuration.component';
import { DashboardComponent } from './components/admin/dashboard/dashboard.component';
import { AdvisorsRequestsComponent } from './components/admin/advisors-requests/advisors-requests.component';
import { AdvisorEditComponent } from './components/advisor/advisor-edit/advisor-edit.component';
import { HomeComponent } from './components/home/home/home.component';
import { TrendingAssetsComponent } from './components/asset/trending-assets/trending-assets.component';
import { TopExpertsComponent } from './components/advisor/top-experts/top-experts.component';

const routes: Routes = [
    { path: '', component: HomeComponent  },
    { path: 'feed', component: AdvicesComponent, canActivate: [AuthRedirect],  },
    { path: 'wallet-login', component: MessageSignatureComponent, canActivate: [AuthRedirect] },
    { path: 'forgot-password-reset', component: ForgotPasswordResetComponent },
    { path: 'change-password', component: ChangePasswordComponent, canActivate:[AuthRedirect] },
    { path: 'top-experts', component: TopExpertsComponent },
    { path: 'expert-details/:id', component: ExpertDetailsComponent, canActivate:[AuthRedirect] },
    { path: 'top-assets', component: ListAssetsComponent, canActivate:[AuthRedirect] },
    { path: 'asset-details/:id', component: AssetDetailsComponent, canActivate:[AuthRedirect] },
    { path: 'new-advice', component: NewAdviceComponent, canActivate:[AuthRedirect] },
    { path: 'referral', component: ReferralComponent, canActivate:[AuthRedirect] },
    { path: 'configuration', component: ConfigurationComponent, canActivate:[AuthRedirect] },
    { path: 'dashboard', component: DashboardComponent, canActivate:[AuthRedirect] },
    { path: 'advisors-requests', component: AdvisorsRequestsComponent, canActivate:[AuthRedirect] },
    { path: 'advisor-edit/:id', component: AdvisorEditComponent, canActivate:[AuthRedirect] },
    { path: 'trending-assets', component: TrendingAssetsComponent },
];

@NgModule({
    exports: [RouterModule],
    imports: [RouterModule.forRoot(routes)]
})

export class AppRoutingModule { }
