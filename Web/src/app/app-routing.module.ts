import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthRedirect } from './providers/authRedirect';
import { MessageSignatureComponent } from './components/account/message-signature/message-signature.component';
import { ExpertDetailsComponent } from './components/advisor/expert-details/expert-details.component';
import { ListAssetsComponent } from './components/asset/list-assets/list-assets.component';
import { AssetDetailsComponent } from './components/asset/asset-details/asset-details.component';
import { FeedComponent } from './components/account/feed/feed.component';
import { DashboardComponent } from './components/admin/dashboard/dashboard.component';
import { AdvisorsRequestsComponent } from './components/admin/advisors-requests/advisors-requests.component';
import { HomeComponent } from './components/home/home/home.component';
import { TopExpertsComponent } from './components/advisor/top-experts/top-experts.component';
import { HotSiteComponent } from './components/hot-site/hot-site.component';
import { BeAnExpertComponent } from './components/hot-site/be-an-expert/be-an-expert.component';
import { ListReportsComponent } from './components/asset/list-reports/list-reports.component';
import { ListEventsComponent } from './components/asset/list-events/list-events.component';

const routes: Routes = [
    { path: '', component: HomeComponent },
    { path: 'hotsite', component: HotSiteComponent },
    { path: 'beexpert', component: BeAnExpertComponent },
    { path: 'feed', component: FeedComponent, canActivate: [AuthRedirect],  },
    { path: 'wallet-login', component: MessageSignatureComponent, canActivate: [AuthRedirect] },
    { path: 'top-experts', component: TopExpertsComponent },
    { path: 'expert-details/:id', component: ExpertDetailsComponent },
    { path: 'top-assets', component: ListAssetsComponent },
    { path: 'asset-details/:id', component: AssetDetailsComponent },
    { path: 'rating-reports', component: ListReportsComponent },
    { path: 'coin-events', component: ListEventsComponent },
    { path: 'dashboard', component: DashboardComponent, canActivate:[AuthRedirect] },
    { path: 'advisors-requests', component: AdvisorsRequestsComponent, canActivate:[AuthRedirect] },
    { path: '**', redirectTo: ''  }
];

@NgModule({
    exports: [RouterModule],
    imports: [RouterModule.forRoot(routes)]
})

export class AppRoutingModule { }
