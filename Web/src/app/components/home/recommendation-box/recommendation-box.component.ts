import { Component, OnInit, Input } from '@angular/core';
import { FeedResponse } from '../../../model/advisor/feedResponse';

@Component({
  selector: 'recommendation-box',
  templateUrl: './recommendation-box.component.html',
  styleUrls: ['./recommendation-box.component.css']
})
export class RecommendationBoxComponent implements OnInit {
  @Input() adviceList: FeedResponse[];
  @Input() adviceType: string;
  displayedColumns: string[] = ['assetCode', 'adviceType', 'advisorName'];

  constructor() { }

  ngOnInit() {
  }

}
