import { Component, OnInit, ViewChild, OnDestroy, SimpleChange } from '@angular/core';
import { AccountService } from '../../../services/account.service';
import { AdvisorDataService } from '../../../services/advisor-data.service';
import { Subscription } from 'rxjs';
import { PositionResponse } from '../../../model/advisor/advisorResponse';
import { OpenPositionsComponent } from '../../trade/portfolio/open-positions/open-positions.component';
import { EventsService } from 'angular-event-service';
import { NavigationService } from '../../../services/navigation.service';

@Component({
  selector: 'mini-view',
  templateUrl: './mini-view.component.html',
  styleUrls: ['./mini-view.component.css']
})
export class MiniViewComponent implements OnInit {
  listOnlyWatchlist = false;
  
  constructor(private accountService: AccountService, private navigationService: NavigationService) { }

  ngOnInit() {
  }

  toggleMarketsWatchlist(){
    this.listOnlyWatchlist = !this.listOnlyWatchlist;
  }

  isLoggedIn(){
    return this.accountService.isLoggedIn();
  }

  goToOpenPositions(){
    this.navigationService.goToPortfolio();
  }
}
