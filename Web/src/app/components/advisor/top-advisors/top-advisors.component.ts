import { Component, OnInit } from '@angular/core';
import { AdvisorService } from '../../../services/advisor.service';
import { AdvisorResponse } from '../../../model/advisor/advisorResponse';

@Component({
  selector: 'top-advisors',
  templateUrl: './top-advisors.component.html',
  styleUrls: ['./top-advisors.component.css']
})
export class TopAdvisorsComponent implements OnInit {
  experts : AdvisorResponse[] = [];

  constructor(private advisorService: AdvisorService) { }

  ngOnInit() {
    this.advisorService.getAdvisors().subscribe(result => this.experts = result);
  }
}
