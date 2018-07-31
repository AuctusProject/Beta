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
  
  advicesData: any=[
    {
      x:1512604800000,
      title: 'BUY'
    },
    {
      x:1513036800000,
      title: 'BUY'
    },
    {
      x:1514100800000,
      title: 'SELL'
    }
  ];
  chartData: any =
  [[1512086400000, 171.05],
  [1512345600000, 169.80],
  [1512432000000, 169.64],
  [1512518400000, 169.01],
  [1512604800000, 169.32],
  [1512691200000, 169.37],
  [1512950400000, 172.67],
  [1513036800000, 171.70],
  [1513123200000, 172.27],
  [1513209600000, 172.22],
  [1513296000000, 173.97],
  [1513555200000, 176.42],
  [1513641600000, 174.54],
  [1513728000000, 174.35],
  [1513814400000, 175.01],
  [1513900800000, 175.01],
  [1514246400000, 170.57]];
  constructor() { }

  ngOnInit() {
    this.assetChart = new StockChart({
      rangeSelector: {
        selected: 1
      },
      credits:{
        enabled: false
      },
      title: {
        text: 'AAPL Stock Price'
      },
      series:[
        {
          name: 'BTC', 
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

  addData(){
    this.chartData.push([1514592000000, 177.01]);
    this.chartData.push([1514937600000, 176.01]);
  }
}
