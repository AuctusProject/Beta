import { Component, OnInit } from '@angular/core';
import { AdvisorService } from '../../../../services/advisor.service';
import { HallOfFameResponse } from '../../../../model/advisor/hallOfFameResponse';
import { Util } from '../../../../util/util';

@Component({
  selector: 'hall-of-fame',
  templateUrl: './hall-of-fame.component.html',
  styleUrls: ['./hall-of-fame.component.css']
})
export class HallOfFameComponent implements OnInit {
  hallOfFame:HallOfFameResponse[];
  displayedColumns = [
    "month",
    "firstPlace",
    "secondPlace",
    "thirdPlace"
  ];

  constructor(private advisorService: AdvisorService) { }

  ngOnInit() {
    this.advisorService.getHallOfFame().subscribe(result => this.hallOfFame = result);
  }

  getFirstPlace(hallOfFame: HallOfFameResponse){
    if(hallOfFame.advisors && hallOfFame.advisors.length >= 1)
      return hallOfFame.advisors[0];
  }

  getSecondPlace(hallOfFame: HallOfFameResponse){
    if(hallOfFame.advisors && hallOfFame.advisors.length >= 2)
      return hallOfFame.advisors[1];
  }

  getThirdPlace(hallOfFame: HallOfFameResponse){
    if(hallOfFame.advisors && hallOfFame.advisors.length >= 3)
      return hallOfFame.advisors[2];
  }

  getMonth(hallOfFame: HallOfFameResponse){
    return Util.GetMonthName(hallOfFame.month).substr(0,3).toUpperCase();
  }
}