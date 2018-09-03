import { Component, OnInit, Input } from '@angular/core';
import { FeedResponse } from '../../../model/advisor/feedResponse';

@Component({
  selector: 'advice-card',
  templateUrl: './advice-card.component.html',
  styleUrls: ['./advice-card.component.css']
})
export class AdviceCardComponent implements OnInit {
  @Input() advice: FeedResponse;

  constructor() { }

  ngOnInit() {
  }

}
