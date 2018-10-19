import { Component, OnInit, Input, ViewChild, ElementRef, OnChanges } from '@angular/core';
//import { StockChart, Highcharts } from 'angular-highcharts';
import { ValuesResponse, AdviceResponse } from '../../../model/asset/assetResponse';
import { Util } from '../../../util/Util';
import { ValueDisplayPipe } from '../../../util/value-display.pipe';
import { AssetService } from '../../../services/asset.service';

@Component({
  selector: 'asset-history-chart',
  templateUrl: './asset-history-chart.component.html',
  styleUrls: ['./asset-history-chart.component.css']
})
export class AssetHistoryChartComponent implements OnChanges {
  @Input() assetId : number;
  @Input() advices : AdviceResponse[];
  @Input() chartTitle?: string;
  //assetChart: StockChart;  
  advicesData: any = [];
  chartData: any = [];
  minAdviceDate?: Date = null;
  mockData: any = [];
  
  constructor(private assetService: AssetService) { }

  ngOnChanges() {
    this.initialize();
  }

  initialize() {
    this.advicesData = [];
    this.chartData = [];
    this.mockData = this.getMockDataForLoading()
    this.minAdviceDate = null;
    // if (this.assetChart) {
    //   this.assetChart.destroy();
    // }
    this.fillAdvicesData();
    this.fillChartData();
  }

  fillChartData(){
    this.createChart();
    // this.assetChart.ref$.subscribe(result => {
    //   this.assetChart.ref.showLoading("Loading data from server...");
    // });

    let queryDate;
    if (this.minAdviceDate) {
      queryDate = new Date(this.minAdviceDate);
      queryDate.setDate(queryDate.getDate() - 7);
    }
    this.assetService.getAssetValues(this.assetId, queryDate).subscribe(ret =>
      {
        for(var i = 0; i < ret.length; i++) {
          this.chartData.push(
            [
              new Date(ret[i].date).getTime(),
              ret[i].value
            ]
          );
        }
        // this.assetChart.ref.series[0].setData(this.chartData);
        // this.assetChart.ref.series[1].setData(this.advicesData);
        // this.assetChart.ref.hideLoading();
      });
  }

  fillAdvicesData(){
    if(!!this.advices) {      
      for(var i =0; i< this.advices.length; i++){
        let date = new Date(this.advices[i].date);
        if (!this.minAdviceDate || date < this.minAdviceDate) {
          this.minAdviceDate = date;
        }
        this.advicesData.push({
          x: date.getTime(),
          title: Util.GetRecommendationTypeDescription(this.advices[i].adviceType),
          text: this.getAdviceFlagText(this.advices[i])
        });
      }
    }
  }

  getAdviceFlagText(advice: AdviceResponse) {
    let text = Util.GetRecommendationTypeDescription(advice.adviceType) + ' at price ' + new ValueDisplayPipe().transform(advice.assetValue);
    if (advice.targetPrice) {
      text += '<br/>Target value: ' +  new ValueDisplayPipe().transform(advice.targetPrice);
    }
    if (advice.stopLoss) {
      text += '<br/>Stop loss: ' + new ValueDisplayPipe().transform(advice.stopLoss);
    }
    if (advice.operationType != 0 && advice.adviceType == 2) {
      text += '<br/>Triggered by ' + Util.GetCloseReasonDescription(advice.operationType);
    } 
    return text;
  }

  createChart(){
    // this.assetChart = new StockChart({
    //   chart:{
    //     zoomType: 'x'
    //   },
    //   plotOptions:{
    //     flags:{
    //       color:'#252525',
    //       fillColor: '#252525',
    //       style: {
    //         color: 'white'
    //       },
    //       states: {
    //           hover: {
    //             color:'#151515',
    //             fillColor: '#151515'
    //           }
    //       }
    //     }
    //   },
    //   rangeSelector: {
    //     enabled:false,
    //   },
    //   title: {
    //     text: this.chartTitle,
    //   },
    //   credits:{
    //     enabled: false
    //   },
    //   series:[
    //     {
    //       name: 'Price', 
    //       data: this.mockData,
    //       id: 'dataseries',
    //     },
    //     {
    //       type: 'flags',
    //       data: [],
    //       onSeries:'dataseries',
          
    //     }
    //   ],
    // });

    this.refresh();
  }

  refresh(){
    // var self = this;
    // setTimeout(() => {self.assetChart.ref.reflow()}, 100);
  }

  getMockDataForLoading() {
    return [[Date.UTC(2018, 1, 14, 19, 59), 100], [Date.UTC(2018, 1, 15, 1, 59), 101.5],
    [Date.UTC(2018, 2, 14, 15, 59), 198.6], [Date.UTC(2018, 2, 14, 19, 59), 200], [Date.UTC(2018, 2, 14, 23, 59), 202.1],
    [Date.UTC(2018, 3, 14, 19, 59), 150], [Date.UTC(2018, 3, 15, 10, 59), 155],
    [Date.UTC(2018, 4, 14, 19, 59), 175], [Date.UTC(2018, 4, 14, 21, 59), 176]];
  }
}
