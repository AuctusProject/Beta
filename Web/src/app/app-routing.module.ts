import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LoginComponent } from './components/account/login/login.component';
import { AuthRedirect } from './providers/authRedirect';
import { ConfirmEmailComponent } from './components/account/confirm-email/confirm-email.component';
import { MessageSignatureComponent } from './components/account/message-signature/message-signature.component';
import { BecomeAdvisorComponent } from './components/advisor/become-advisor/become-advisor.component';
import { ForgotPasswordComponent } from './components/account/forgot-password/forgot-password.component';
import { ForgotPasswordResetComponent } from './components/account/forgot-password-reset/forgot-password-reset.component';
import { ChangePasswordComponent } from './components/account/change-password/change-password.component';
import { ListAdvisorsComponent } from './components/advisor/list-advisors/list-advisors.component';
import { RegisterComponent } from './components/account/register/register.component';
import { AdvisorDetailsComponent } from './components/advisor/advisor-details/advisor-details.component';
import { ListAssetsComponent } from './components/asset/list-assets/list-assets.component';
import { AssetDetailsComponent } from './components/asset/asset-details/asset-details.component';
import { AdvicesComponent } from './components/advisor/advices/advices.component';
import { NewAdviceComponent } from './components/advisor/new-advice/new-advice.component';
import { ReferralComponent } from './components/account/referral/referral.component';
import { ConfigurationComponent } from './components/account/configuration/configuration.component';

const routes: Routes = [
    { path: '', redirectTo: 'feed', pathMatch: 'full' },
    { path: 'login', component: LoginComponent },
    { path: 'register', component: RegisterComponent },
    { path: 'feed', component: AdvicesComponent, canActivate: [AuthRedirect],  },
    { path: 'confirm-email', component: ConfirmEmailComponent, canActivate: [AuthRedirect] },
    { path: 'wallet-login', component: MessageSignatureComponent, canActivate: [AuthRedirect] },
    { path: 'become-advisor', component: BecomeAdvisorComponent, canActivate: [AuthRedirect] },
    { path: 'forgot-password', component: ForgotPasswordComponent },
    { path: 'forgot-password-reset', component: ForgotPasswordResetComponent },
    { path: 'change-password', component: ChangePasswordComponent, canActivate:[AuthRedirect] },
    { path: 'list-advisors', component: ListAdvisorsComponent, canActivate:[AuthRedirect] },
    { path: 'advisor-details/:id', component: AdvisorDetailsComponent, canActivate:[AuthRedirect] },
    { path: 'list-assets', component: ListAssetsComponent, canActivate:[AuthRedirect] },
    { path: 'asset-details/:id', component: AssetDetailsComponent, canActivate:[AuthRedirect] },
    { path: 'new-advice', component: NewAdviceComponent, canActivate:[AuthRedirect] },
    { path: 'referral', component: ReferralComponent, canActivate:[AuthRedirect] },
    { path: 'configuration', component: ConfigurationComponent, canActivate:[AuthRedirect] }
];

@NgModule({
    exports: [RouterModule],
    imports: [RouterModule.forRoot(routes)]
})

export class AppRoutingModule { }
