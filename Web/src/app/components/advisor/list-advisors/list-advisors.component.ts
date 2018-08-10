import { Component, OnInit } from '@angular/core';
import { AdvisorResponse } from '../../../model/advisor/advisorResponse';
import { AdvisorService } from '../../../services/advisor.service';
import { Sortable } from '../../../util/Util'
@Component({
  selector: 'list-advisors',
  templateUrl: './list-advisors.component.html',
  styleUrls: ['./list-advisors.component.css']
})
export class ListAdvisorsComponent implements OnInit {
  advisors : AdvisorResponse[] = [];
  selectedSortOption: number;
  sortOptions = [
    {value:1, name:"Ranking"},
    {value:2, name:"Return Rate"},
    {value:3, name:"Success Rate"},
    {value:4, name:"# of Followers"},
    {value:5, name:"Name"},
  ];
  constructor(private advisorService: AdvisorService) { }

  ngOnInit() {
    this.advisorService.getAdvisors().subscribe(result => this.advisors = result);
  }

  onSortOptionChanged(){
    if(this.selectedSortOption == 1){
      this.sortByRanking();
    }
    else if(this.selectedSortOption == 2){
      this.sortByReturnRate();
    }
    else if(this.selectedSortOption == 3){
      this.sortBySuccessRate();
    }
    else if(this.selectedSortOption == 4){
      this.sortByFollowers();
    }
    else if(this.selectedSortOption == 5){
      this.sortByName();
    }
  }

  sortByName(){
    Sortable.sort<AdvisorResponse>(this.advisors, a => a.name);
  }
  sortByRanking(){
    Sortable.sort<AdvisorResponse>(this.advisors, a => a.ranking);
  }
  sortByFollowers(){
    Sortable.sort<AdvisorResponse>(this.advisors, a => a.numberOfFollowers, "DESC");
  }
  sortByReturnRate(){
    Sortable.sort<AdvisorResponse>(this.advisors, a => a.averageReturn, "DESC");
  }
  sortBySuccessRate(){
    Sortable.sort<AdvisorResponse>(this.advisors, a => a.successRate, "DESC");
  }
}
