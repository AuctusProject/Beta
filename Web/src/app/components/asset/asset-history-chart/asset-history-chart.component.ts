import { Component, OnInit, Input } from '@angular/core';
import { StockChart, Chart } from 'angular-highcharts';
import { AssetValue } from '../../../model/asset/AssetValue';
import { Advice } from '../../../model/advisor/Advice';


@Component({
  selector: 'asset-history-chart',
  templateUrl: './asset-history-chart.component.html',
  styleUrls: ['./asset-history-chart.component.css']
})
export class AssetHistoryChartComponent implements OnInit {
  @Input() assetValues : AssetValue[];
  @Input() advices : Advice[];
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
    for(var i =0;i<this.assetValues.length;i++){
      this.chartData.push([this.assetValues[i].date.getUTCMilliseconds(), this.assetValues[i].value]);
    }    
  }

  fillAdvicesData(){
    if(this.advices){
      for(var i =0;i<this.advices.length;i++){
        this.advicesData.push({
          x: this.advices[i].creationDate.getUTCMilliseconds(),
          title: this.advices[i].type
        });
      }
    }
  }

  createChart(){
    this.assetChart = new StockChart({
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
