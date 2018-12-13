import { Component, OnInit, Input } from '@angular/core';
import { Util } from '../../../../util/util';

@Component({
  selector: 'prize-box',
  templateUrl: './prize-box.component.html',
  styleUrls: ['./prize-box.component.css']
})
export class PrizeBoxComponent implements OnInit {
  @Input() theme?:string = "dark";
  @Input() rank:number;

  constructor() { }

  ngOnInit() {
  }

  getOrdinalRank(){
    return Util.GetNumberWithOrdinalSuffix(this.rank);
  }

  getPrize(){
    if(this.rank == 1){
      return "3,000";
    }
    else if(this.rank == 2){
      return "1,500";
    }
    else{
      return "500";
    }
  }
}