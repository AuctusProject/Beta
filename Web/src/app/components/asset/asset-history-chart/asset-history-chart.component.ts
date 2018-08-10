import { Component, OnInit, Input } from '@angular/core';
import { StockChart, Chart } from 'angular-highcharts';
import { ValuesResponse, AdviceResponse } from '../../../model/asset/assetResponse';
import { Util } from '../../../util/Util';


@Component({
  selector: 'asset-history-chart',
  templateUrl: './asset-history-chart.component.html',
  styleUrls: ['./asset-history-chart.component.css']
})
export class AssetHistoryChartComponent implements OnInit {
  @Input() assetValues : ValuesResponse[];
  @Input() advices : AdviceResponse[];
  assetChart: StockChart;  
  advicesData: any = [];
  chartData: any = [];
  
  constructor() { }

  ngOnInit() {
     this.fillChartData();
     this.fillAdvicesData();
     this.createChart();
  }

  fillChartData(){
    for(var i =0;i<this.assetValues.length && i< 50;i++){
      this.chartData.push(
        [Date.parse(this.assetValues[i].date),
        this.assetValues[i].value]
      );
    }    
  }

  fillAdvicesData(){
    if(this.advices){
      for(var i =0;i<this.advices.length;i++){
        this.advicesData.push({
          x: Date.parse(this.advices[i].date),
          title: Util.GetRecommendationTypeDescription(this.advices[i].adviceType)
        });
      }
    }
  }

  createChart(){
    this.assetChart = new StockChart({
      chart:{
        zoomType: 'x'
      },
      rangeSelector: {
        selected: 1
      },
      credits:{
        enabled: false
      },
      series:[
        {
          name: 'Price', 
          data: this.chartData,
          id: 'dataseries'
        },
        {
          type: 'flags',
          data: this.advicesData,
          onSeries:'dataseries'
        }
      ]
    }); 
  }
}
