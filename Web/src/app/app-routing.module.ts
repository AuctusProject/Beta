import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TopAdvisorsComponent } from './components/advisor/top-advisors/top-advisors.component';
import { LoginComponent } from './components/account/login/login.component';
import { AuthGuard } from './providers/authGuard';
import { AuthRedirect } from './providers/authRedirect';
import { ConfirmEmailComponent } from './components/account/confirm-email/confirm-email.component';

const routes: Routes = [
    { path: '', redirectTo: 'feed', pathMatch: 'full' },
    { path: 'top-advisors', component: TopAdvisorsComponent, canActivate: [AuthGuard]  },
    { path: 'login', component: LoginComponent, canActivate: [AuthRedirect] },
    { path: 'feed', component: LoginComponent, canActivate: [AuthGuard] },
    { path: 'confirm-email', component: ConfirmEmailComponent, canActivate: [AuthGuard] },
];

@NgModule({
    exports: [RouterModule],
    imports: [RouterModule.forRoot(routes)]
})

export class AppRoutingModule { }
