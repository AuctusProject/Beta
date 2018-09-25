import { Component, OnInit, Input, ViewChild, ElementRef, OnChanges } from '@angular/core';
import { StockChart, Highcharts } from 'angular-highcharts';
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
  assetChart: StockChart;  
  advicesData: any = [];
  chartData: any = [];
  minAdviceDate?: Date = null;
  
  constructor(private assetService: AssetService) { }

  ngOnChanges() {
    this.initialize();
  }

  initialize() {
    this.advicesData = [];
    this.chartData = [];
    this.minAdviceDate = null;
    if (this.assetChart) {
      this.assetChart.destroy();
    }
    this.fillAdvicesData();
    this.fillChartData();
  }

  fillChartData(){
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
        this.createChart();
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
          text: Util.GetRecommendationTypeDescription(this.advices[i].adviceType) + ' at price ' + new ValueDisplayPipe().transform(this.advices[i].assetValue)
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
          data: this.chartData,
          id: 'dataseries',
        },
        {
          type: 'flags',
          data: this.advicesData,
          onSeries:'dataseries',
          
        }
      ],
    });

    this.refresh();
  }

  refresh(){
    var self = this;
    setTimeout(() => {self.assetChart.ref.reflow()}, 100);
  }
}
