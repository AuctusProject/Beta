import { Component, OnInit, Input } from '@angular/core';
import { AdvisorResponse } from '../../../model/advisor/advisorResponse';

@Component({
  selector: 'advisor-card',
  templateUrl: './advisor-card.component.html',
  styleUrls: ['./advisor-card.component.css']
})
export class AdvisorCardComponent implements OnInit {
  @Input() advisor: AdvisorResponse;
  
  constructor() { }

  ngOnInit() {
  }

}
