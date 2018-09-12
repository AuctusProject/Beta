import { Component, OnInit, Input, ViewChild, ElementRef } from '@angular/core';
import { StockChart, Highcharts } from 'angular-highcharts';
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
  @Input() chartTitle?: string;
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
    if(!!this.assetValues) {
      for(var i = 0; i < this.assetValues.length; i++){
        this.chartData.push(
          [Date.parse(this.assetValues[i].date),
          this.assetValues[i].value]
        );
      }    
    }
  }

  fillAdvicesData(){
    if(!!this.advices) {
      for(var i =0; i< this.advices.length; i++){
        this.advicesData.push({
          x: Date.parse(this.advices[i].date),
          title: Util.GetRecommendationTypeDescription(this.advices[i].adviceType),
          text: Util.GetRecommendationTypeDescription(this.advices[i].adviceType)
        });
      }
    }
  }

  createChart(){
    this.assetChart = new StockChart({
      chart:{
        zoomType: 'x'
      },
      plotOptions:{
        flags:{
          color:'#2c8b8b',
          fillColor: '#2c8b8b'
        }
      },
      rangeSelector: {
        enabled:false,
      },
      title: {
        text: this.chartTitle,
      },
      credits:{
        enabled: false
      },
      series:[
        {
          name: 'Price', 
          data: this.chartData,
          id: 'dataseries',
        },
        {
          type: 'flags',
          data: this.advicesData,
          onSeries:'dataseries',
          
        }
      ]
    });

    this.refresh();
  }

  refresh(){
    var self = this;
    setTimeout(() => {self.assetChart.ref.reflow()}, 100);
  }
}
