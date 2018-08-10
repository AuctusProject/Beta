import { Component, OnInit } from '@angular/core';
import { AdvisorService } from '../../../services/advisor.service';
import { AdvisorResponse } from '../../../model/advisor/advisorResponse';

@Component({
  selector: 'top-advisors',
  templateUrl: './top-advisors.component.html',
  styleUrls: ['./top-advisors.component.css']
})
export class TopAdvisorsComponent implements OnInit {
  advisors : AdvisorResponse[];
  constructor(private advisorService: AdvisorService) { }

  ngOnInit() {
    this.getAdvisors();
  }

  getAdvisors(){
    this.advisorService.getAdvisors().subscribe(
      advisors => this.advisors = advisors
    );
  }
}
