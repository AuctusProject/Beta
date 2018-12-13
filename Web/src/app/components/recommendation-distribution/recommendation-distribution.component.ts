import { Component, OnInit, Input, OnChanges, ViewChild, Inject, PLATFORM_ID } from '@angular/core';
import { RecommendationDistributionResponse } from '../../model/recommendationDistributionResponse';
import { Util } from '../../util/util';
import { isPlatformBrowser } from '@angular/common';

@Component({
  selector: 'recommendation-distribution',
  templateUrl: './recommendation-distribution.component.html',
  styleUrls: ['./recommendation-distribution.component.css']
})
export class RecommendationDistributionComponent implements OnInit, OnChanges {
chartLabels:string[] = ['buy', 'sell', 'close'];
chartColors:Array<any> = [{backgroundColor: ['#3ed142','#d13e3e','#383838']}];
chartData:number[] = [0,0,1];
chartType:string = 'doughnut';
chartOptions:any = this.getChartOptions();

mouseHovered:boolean = false;

  @Input() data : RecommendationDistributionResponse[];
  @Input() showRight: boolean = true;
  totalBuy = 0;
  totalSell = 0;
  totalClose = 0;
  enableTooltip = true;
  constructor(@Inject(PLATFORM_ID) private platformId: Object) { }

  ngOnInit() {
    this.fillPieData();
  }

  ngOnChanges() {
    this.fillPieData();
  }

  fillPieData(){
    this.totalBuy = 0;
    this.totalSell = 0;
    this.totalClose = 0;
    if (this.data && this.data.length) {
        for(var i = 0; i < this.data.length; i++){
            if(this.data[i].type == Util.BUY)
                this.totalBuy = this.data[i].total;
            if(this.data[i].type == Util.SELL)
                this.totalSell = this.data[i].total;
            if(this.data[i].type == Util.CLOSE)
                this.totalClose = this.data[i].total;
        }

        this.chartData = [ this.totalBuy, this.totalSell, this.totalClose];
    }
    else {
        this.chartData = [0,0,1];
    }
  }

  getChartOptions(){
      return {
        legend:{display:false},
        cutoutPercentage: 75,
        animation:false, 
        elements: {
            arc: {
                borderWidth: 0
            }
        },
        tooltips: {
            enabled: false
        }
    };
  }
}
