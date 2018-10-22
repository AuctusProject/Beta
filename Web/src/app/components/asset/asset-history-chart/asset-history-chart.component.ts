import { Component, Input, OnChanges, AfterViewChecked, SimpleChanges, ViewChild, Inject, PLATFORM_ID } from '@angular/core';
//import {stockChart} from 'highcharts/highstock';
import { AdviceResponse } from '../../../model/asset/assetResponse';
import { Util } from '../../../util/Util';
import { ValueDisplayPipe } from '../../../util/value-display.pipe';
import { AssetService } from '../../../services/asset.service';
import { isPlatformBrowser } from '@angular/common';

@Component({
  selector: 'asset-history-chart',
  templateUrl: './asset-history-chart.component.html',
  styleUrls: ['./asset-history-chart.component.css']
})
export class AssetHistoryChartComponent implements OnChanges {
  @ViewChild("ChartContainer") chartContainer: any;
  @Input() assetId : number;
  @Input() advices : AdviceResponse[];
  @Input() chartTitle?: string;
  assetChart: any;  
  advicesData: any = [];
  chartData: any = [];
  minAdviceDate?: Date = null;
  mockData: any = [];

  constructor(private assetService: AssetService, @Inject(PLATFORM_ID) private platformId: Object) { }

  ngOnChanges(changes: SimpleChanges) {
    this.initialize();
  }

  initialize() {
    this.advicesData = [];
    this.chartData = [];
    this.mockData = this.getMockDataForLoading()
    this.minAdviceDate = null;
    this.fillAdvicesData();
    this.fillChartData();
  }

  fillChartData(){
    this.createChart();
    if (this.assetChart) {
      this.assetChart.showLoading("Loading data from server...");
    }

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
        if (this.assetChart) {
          this.assetChart.series[0].setData(this.chartData);
          this.assetChart.series[1].setData(this.advicesData);
          this.assetChart.hideLoading();
        }
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
    if(isPlatformBrowser(this.platformId) && window && (<any>window).Highcharts){
      this.assetChart = (<any>window).Highcharts.stockChart(this.chartContainer.nativeElement,
        {
        chart:{
          zoomType: 'x'
        },
        plotOptions:{
          flags:{
            color:'#252525',
            fillColor: '#252525',
            style: {
              color: 'white'
            },
            states: {
                hover: {
                  color:'#151515',
                  fillColor: '#151515'
                }
            }
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
            data: this.mockData,
            id: 'dataseries',
          },
          {
            type: 'flags',
            data: [],
            onSeries:'dataseries',
            
          }
        ],
      });
    }
    this.refresh();
  }

  refresh(){
    var self = this;
    setTimeout(() => {
      if (this.assetChart) {
        this.assetChart.reflow()
      }
    }, 100);
  }

  getMockDataForLoading() {
    return [[Date.UTC(2018, 1, 14, 19, 59), 100], [Date.UTC(2018, 1, 15, 1, 59), 101.5],
    [Date.UTC(2018, 2, 14, 15, 59), 198.6], [Date.UTC(2018, 2, 14, 19, 59), 200], [Date.UTC(2018, 2, 14, 23, 59), 202.1],
    [Date.UTC(2018, 3, 14, 19, 59), 150], [Date.UTC(2018, 3, 15, 10, 59), 155],
    [Date.UTC(2018, 4, 14, 19, 59), 175], [Date.UTC(2018, 4, 14, 21, 59), 176]];
  }
}
