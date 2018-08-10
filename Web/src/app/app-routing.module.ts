import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TopAdvisorsComponent } from './components/advisor/top-advisors/top-advisors.component';
import { LoginComponent } from './components/account/login/login.component';
import { AuthGuard } from './providers/authGuard';
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

const routes: Routes = [
    { path: '', redirectTo: 'feed', pathMatch: 'full' },
    { path: 'top-advisors', component: TopAdvisorsComponent, canActivate: [AuthRedirect]  },
    { path: 'login', component: LoginComponent },
    { path: 'register', component: RegisterComponent },
    { path: 'feed', component: LoginComponent, canActivate: [AuthRedirect],  },
    { path: 'confirm-email', component: ConfirmEmailComponent, canActivate: [AuthRedirect] },
    { path: 'wallet-login', component: MessageSignatureComponent, canActivate: [AuthRedirect] },
    { path: 'become-advisor', component: BecomeAdvisorComponent, canActivate: [AuthRedirect] },
    { path: 'forgot-password', component: ForgotPasswordComponent },
    { path: 'forgot-password-reset', component: ForgotPasswordResetComponent },
    { path: 'change-password', component: ChangePasswordComponent,canActivate:[AuthRedirect] },
    { path: 'list-advisors', component: ListAdvisorsComponent,canActivate:[AuthRedirect] },
    { path: 'advisor-details/:id', component: AdvisorDetailsComponent,canActivate:[AuthRedirect] },
];

@NgModule({
    exports: [RouterModule],
    imports: [RouterModule.forRoot(routes)]
})

export class AppRoutingModule { }
