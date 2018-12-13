import { Component, OnInit, Input } from '@angular/core';
import { Util } from '../../../util/util';

@Component({
  selector: 'general-recommendation-tag',
  templateUrl: './general-recommendation-tag.component.html',
  styleUrls: ['./general-recommendation-tag.component.css']
})
export class GeneralRecommendationTagComponent implements OnInit {
  @Input() value: number;
  
  constructor() { }

  ngOnInit() {
  }

  getGeneralRecommendation(){
    return Util.GetGeneralRecommendationDescription(this.value);
  }
}
