import { Component, OnInit, ViewChild } from '@angular/core';
import { Subscription } from 'rxjs';
import { PerfomanceOpenPositionsComponent, OpenPositionsType } from '../performance/perfomance-open-positions/perfomance-open-positions.component';
import { AccountService } from '../../../../services/account.service';
import { AdvisorDataService } from '../../../../services/advisor-data.service';
import { EventsService } from 'angular-event-service';

@Component({
  selector: 'portfolio-mini-view',
  templateUrl: './portfolio-mini-view.component.html',
  styleUrls: ['./portfolio-mini-view.component.css']
})
export class PortfolioMiniViewComponent implements OnInit {
  miniViewType = OpenPositionsType.miniView;
  userId?: number;

  allOpenPositionResponseSubscription: Subscription;
  
  @ViewChild("Open") Open: PerfomanceOpenPositionsComponent;
  
  constructor(private accountService: AccountService,
    private advisorDataService: AdvisorDataService,
    private eventsService: EventsService) { }

  ngOnInit() {
    this.eventsService.on("onLogin", () => this.onLogin());
    this.onLogin()
  }

  onLogin(){
    var loginData = this.accountService.getLoginData();
    if(loginData){
      this.userId = loginData.id;
      this.initialize();
    }
  }

  initialize() {
    if (this.userId) {
      this.advisorDataService.initialize(this.userId);
      this.allOpenPositionResponseSubscription = this.advisorDataService.listAllOpenPositions().subscribe(
        ret => {
          this.Open.setOpenPositions(ret);
        });

      this.eventsService.on("onUpdateAdvisor", () => this.refreshAll());
    }
  }

  refreshAll() {
    this.advisorDataService.refresh();
  }

  ngOnDestroy(){
    if (this.allOpenPositionResponseSubscription) this.allOpenPositionResponseSubscription.unsubscribe();
    this.advisorDataService.destroy();
  }
}
