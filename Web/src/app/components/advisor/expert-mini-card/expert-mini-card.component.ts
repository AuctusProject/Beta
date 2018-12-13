import { Component, OnInit, Input } from '@angular/core';
import { AdvisorResponse } from '../../../model/advisor/advisorResponse';

@Component({
  selector: 'expert-mini-card',
  templateUrl: './expert-mini-card.component.html',
  styleUrls: ['./expert-mini-card.component.css']
})
export class ExpertMiniCardComponent implements OnInit {
  @Input() expert:AdvisorResponse;
  @Input() layoutAlign?:string;
  constructor() { }

  ngOnInit() {
  }

}
