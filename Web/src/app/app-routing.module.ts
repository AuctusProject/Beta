import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthRedirect } from './providers/authRedirect';
import { MessageSignatureComponent } from './components/account/message-signature/message-signature.component';
import { ExpertDetailsComponent } from './components/advisor/expert-details/expert-details.component';
import { ListAssetsComponent } from './components/asset/list-assets/list-assets.component';
import { AssetDetailsComponent } from './components/asset/asset-details/asset-details.component';
import { AdvicesComponent } from './components/advisor/advices/advices.component';
import { DashboardComponent } from './components/admin/dashboard/dashboard.component';
import { AdvisorsRequestsComponent } from './components/admin/advisors-requests/advisors-requests.component';
import { HomeComponent } from './components/home/home/home.component';
import { TopExpertsComponent } from './components/advisor/top-experts/top-experts.component';
import { HotSiteComponent } from './components/hot-site/hot-site.component';

const routes: Routes = [
    { path: 'hotsite', component: HotSiteComponent  },
    { path: '', component: HomeComponent  },
    { path: 'feed', component: AdvicesComponent, canActivate: [AuthRedirect],  },
    { path: 'wallet-login', component: MessageSignatureComponent, canActivate: [AuthRedirect] },
    { path: 'top-experts', component: TopExpertsComponent },
    { path: 'expert-details/:id', component: ExpertDetailsComponent, canActivate:[AuthRedirect] },
    { path: 'top-assets', component: ListAssetsComponent, canActivate:[AuthRedirect] },
    { path: 'asset-details/:id', component: AssetDetailsComponent, canActivate:[AuthRedirect] },
    { path: 'dashboard', component: DashboardComponent, canActivate:[AuthRedirect] },
    { path: 'advisors-requests', component: AdvisorsRequestsComponent, canActivate:[AuthRedirect] },
    { path: '**', redirectTo: ''  }
];

@NgModule({
    exports: [RouterModule],
    imports: [RouterModule.forRoot(routes)]
})

export class AppRoutingModule { }
