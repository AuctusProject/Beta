import { Component, OnInit } from '@angular/core';
import { AdvisorService } from '../../../services/advisor.service';
import { Advisor } from '../../../model/advisor/advisor';

@Component({
  selector: 'top-advisors',
  templateUrl: './top-advisors.component.html',
  styleUrls: ['./top-advisors.component.css']
})
export class TopAdvisorsComponent implements OnInit {
  advisors : Advisor[];
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
