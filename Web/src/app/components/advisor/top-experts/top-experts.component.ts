import { Component, OnInit } from '@angular/core';
import { Title, Meta } from '@angular/platform-browser';
import { AdvisorService } from '../../../services/advisor.service';
import { AdvisorResponse } from '../../../model/advisor/advisorResponse';
import { AccountService } from '../../../services/account.service';
import { FollowUnfollowType } from '../../util/follow-unfollow/follow-unfollow.component';
import { ExpertsTableType } from '../experts-table/experts-table.component';

@Component({
  selector: 'top-experts',
  templateUrl: './top-experts.component.html',
  styleUrls: ['./top-experts.component.css']
})
export class TopExpertsComponent implements OnInit {
  experts: AdvisorResponse[];

  constructor(private advisorService: AdvisorService,
    private titleService: Title,
    private metaTagService: Meta) { }

  ngOnInit() {
    this.titleService.setTitle("Auctus - Top Cryptocurrency Traders");
    this.metaTagService.updateTag({name: 'description', content: "Find the best performing Crypto Analysts."});
    this.advisorService.getAdvisors().subscribe(result => {
      this.experts = result;
    });
  }
}
