import { Component, OnInit, Input } from '@angular/core';
import { Advisor } from '../../../model/advisor/advisor';

@Component({
  selector: 'advisor-card',
  templateUrl: './advisor-card.component.html',
  styleUrls: ['./advisor-card.component.css']
})
export class AdvisorCardComponent implements OnInit {
  @Input() advisor: Advisor;
  
  constructor() { }

  ngOnInit() {
  }

}
