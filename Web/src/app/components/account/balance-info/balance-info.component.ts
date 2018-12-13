import { Component, OnInit, Input, OnChanges, OnDestroy } from '@angular/core';
import { AdvisorResponse } from '../../../model/advisor/advisorResponse';

@Component({
  selector: 'balance-info',
  templateUrl: './balance-info.component.html',
  styleUrls: ['./balance-info.component.css']
})
export class BalanceInfoComponent implements OnInit, OnChanges, OnDestroy {
  @Input() advisor : AdvisorResponse;
  constructor() { }

  ngOnInit(){
  }

  ngOnChanges() {
    // if(this.advisor){
    //   this.advisorDataService.initializeWithResponse(this.advisor);
    //   this.advisorDataService.advisorResponse().subscribe(ret => this.advisor = ret);
    // }
    // else{
    //   this.advisorDataService.destroy();
    // }
  }

  ngOnDestroy(){
    // this.advisorDataService.destroy();
  }
}
