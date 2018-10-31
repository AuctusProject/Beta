import { Component, OnInit, Input, OnChanges, ViewChild, Inject, PLATFORM_ID } from '@angular/core';
import { RecommendationDistributionResponse } from '../../model/recommendationDistributionResponse';
import { Util } from '../../util/Util';
import { isPlatformBrowser } from '@angular/common';

@Component({
  selector: 'recommendation-distribution',
  templateUrl: './recommendation-distribution.component.html',
  styleUrls: ['./recommendation-distribution.component.css']
})
export class RecommendationDistributionComponent implements OnInit, OnChanges {
  pieChart: any;
  @ViewChild("PieChartContainer") pieChartContainer: any; 
  @Input() data : RecommendationDistributionResponse[];
  @Input() showTitle : boolean;
  pieData: any;
  totalBuy = 0;
  totalSell = 0;
  totalClose = 0;
  totalRecommendations = 0;
  constructor(@Inject(PLATFORM_ID) private platformId: Object) { }

  ngOnInit() {
    this.fillPieData();
    this.createPieChart();
  }

  ngOnChanges() {
    this.fillPieData();
    this.createPieChart();
  }

  fillPieData(){
    this.pieData = [];
    this.totalBuy = 0;
    this.totalSell = 0;
    this.totalClose = 0;
    this.totalRecommendations = 0;
    if (this.data) {
        for(var i = 0; i < this.data.length; i++){
            this.pieData.push({
                name: Util.GetRecommendationTypeDescription(this.data[i].type),
                y: this.data[i].total,
                color: Util.GetRecommendationTypeColor(this.data[i].type)
            });

            if(this.data[i].type == Util.BUY)
                this.totalBuy = this.data[i].total;
            if(this.data[i].type == Util.SELL)
                this.totalSell = this.data[i].total;
            if(this.data[i].type == Util.CLOSE)
                this.totalClose = this.data[i].total;
        }
        this.totalRecommendations = this.totalBuy + this.totalSell + this.totalClose;
    }
  }

  createPieChart(){
    if(isPlatformBrowser(this.platformId) && window && (<any>window).Highcharts){
        this.pieChart = (<any>window).Highcharts.chart({
            chart: {
                renderTo: this.pieChartContainer.nativeElement,
                backgroundColor: null,
                plotBorderWidth: null,
                plotShadow: false,
                type: 'pie'
        },
        credits:{
            enabled: false
        },
        legend:{enabled:false},
        title: {
            text: this.showTitle ? "<b>"+ this.totalRecommendations+"</b><br />Signals" : null,
            align: 'center',
            verticalAlign: 'middle',
            y: -4,
            style: { "color": "#ffffff", "fontSize": "8px" }
        },
        tooltip: {
            pointFormat: '<b>{point.percentage:.1f}%</b>'
        },
        plotOptions: {
            pie: {
                allowPointSelect: false,
                cursor: 'pointer',
                dataLabels: {
                    enabled: false,
                    format: '<b>{point.name}</b>: {point.percentage:.1f}%',
                    style: {
                        color: 'black'
                    }
                },
                borderColor: null,
                showInLegend: true,
                center: ['50%', '50%']
            }
        },
        series: [{
          name: 'Recommendations',
            innerSize: '80%',
            data: this.pieData
        }]
        });
    }
  }
}
