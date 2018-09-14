import { Component, OnInit, Input } from '@angular/core';
import { FeedResponse } from '../../../model/advisor/feedResponse';
import { CONFIG } from '../../../services/config.service';

@Component({
  selector: 'advice-card',
  templateUrl: './advice-card.component.html',
  styleUrls: ['./advice-card.component.css']
})
export class AdviceCardComponent implements OnInit {
  @Input() advice : FeedResponse;

  constructor() { }

  ngOnInit() {
  }

  
  getAdvisorImgUrl(){
    return CONFIG.profileImgUrl.replace("{id}", this.advice.advisorUrlGuid);
  }
}
