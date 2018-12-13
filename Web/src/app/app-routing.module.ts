import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthRedirect } from './providers/authRedirect';
import { ListAssetsComponent } from './components/asset/list-assets/list-assets.component';
import { AssetDetailsComponent } from './components/asset/asset-details/asset-details.component';
import { HomeComponent } from './components/home/home/home.component';
import { TopExpertsComponent } from './components/advisor/top-experts/top-experts.component';
import { PortfolioComponent } from './components/trade/portfolio/portfolio.component';
import { WatchlistComponent } from './components/account/watchlist/watchlist.component';
import { TradingContestComponent } from './components/advisor/trading-contest/trading-contest.component';
import { ReferralDetailsComponent } from './components/account/referral-details/referral-details.component';

const routes: Routes = [
    { path: '', component: HomeComponent },
    { path: 'top-traders', component: TopExpertsComponent },
    { path: 'top-traders/:id', component: PortfolioComponent },
    { path: 'trade-markets', component: ListAssetsComponent },
    { path: 'trade-markets/:id', component: AssetDetailsComponent },
    { path: 'portfolio', component: PortfolioComponent, canActivate: [AuthRedirect]},
    { path: 'watchlist', component: WatchlistComponent, canActivate: [AuthRedirect]},
    { path: 'trading-contest', component: TradingContestComponent},
    { path: 'invite', component: ReferralDetailsComponent, canActivate: [AuthRedirect]},
    { path: '**', redirectTo: ''  }
];

@NgModule({
    exports: [RouterModule],
    imports: [RouterModule.forRoot(routes,{
        scrollPositionRestoration: 'enabled',
      })]
})

export class AppRoutingModule { }
