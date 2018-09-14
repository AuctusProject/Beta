import { Component, OnInit, Input } from '@angular/core';
import { AdvisorService } from '../../../services/advisor.service';
import { AdvisorResponse } from '../../../model/advisor/advisorResponse';
import { NavigationService } from '../../../services/navigation.service';
import { Util } from '../../../util/Util';
import { AccountService } from '../../../services/account.service';
import { ModalService } from '../../../services/modal.service';

@Component({
  selector: 'top-experts',
  templateUrl: './top-experts.component.html',
  styleUrls: ['./top-experts.component.css']
})
export class TopExpertsComponent implements OnInit {
  @Input() resultsLimit?: number;
  @Input() hideFilters?: boolean;
  experts : AdvisorResponse[] = [];
  expertsResponse : AdvisorResponse[];
  showAdvisorButton: boolean = false;
  
  pageSize = 12;
  currentPage = 1;
  
  selectedSortOption: number = 1;
  sortOptions = [
    {value:1, name:"Ranking"},
    {value:2, name:"Return Rate"},
    {value:3, name:"Success Rate"},
    {value:4, name:"Total Followers"},
    {value:5, name:"Name"},
  ];

  constructor(private advisorService: AdvisorService, 
    public accountService: AccountService,
    private navigationService: NavigationService,
    private modalService: ModalService) { }

  ngOnInit() {
    this.showAdvisorButton = this.accountService.isLoggedIn() && this.accountService.getLoginData().isAdvisor;
    this.advisorService.getAdvisors().subscribe(result => {
      this.expertsResponse = result;
      if(this.resultsLimit != null && result != null){
        this.experts = result.slice(0,this.resultsLimit);
      }
      else{
        this.setVisibleExperts();
      }
    });
  }

  onNewAdviceClick() {
    this.modalService.setNewAdvice();
  }

  onEditProfileClick() {
    this.modalService.setEditAdvisor(this.accountService.getLoginData().id);
  }

  loadMoreExperts(){
    this.currentPage++;
    this.setVisibleExperts();
  }

  hasMoreExperts(){
    return this.expertsResponse != null && this.experts.length != this.expertsResponse.length;
  }

  setVisibleExperts(){
    var numberOfExpertsToShow = this.pageSize * this.currentPage;
    this.experts = this.expertsResponse.slice(0, numberOfExpertsToShow);
  }

  goToTopExperts(){
    this.navigationService.goToTopExperts();
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
    this.setVisibleExperts();
  }

  sortByName(){
    Util.Sort<AdvisorResponse>(this.expertsResponse, a => a.name);
  }
  sortByRanking(){
    Util.Sort<AdvisorResponse>(this.expertsResponse, a => a.ranking);
  }
  sortByFollowers(){
    Util.Sort<AdvisorResponse>(this.expertsResponse, a => a.numberOfFollowers, "DESC");
  }
  sortByReturnRate(){
    Util.Sort<AdvisorResponse>(this.expertsResponse, a => a.averageReturn, "DESC");
  }
  sortBySuccessRate(){
    Util.Sort<AdvisorResponse>(this.expertsResponse, a => a.successRate, "DESC");
  }
}
