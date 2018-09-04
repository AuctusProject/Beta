import { Component, OnInit, Input } from '@angular/core';
import { Chart } from 'angular-highcharts';
import { RecommendationDistributionResponse } from '../../model/recommendationDistributionResponse';
import { Util } from '../../util/Util';

@Component({
  selector: 'recommendation-distribution',
  templateUrl: './recommendation-distribution.component.html',
  styleUrls: ['./recommendation-distribution.component.css']
})
export class RecommendationDistributionComponent implements OnInit {
  pieChart: Chart;
  @Input() data : RecommendationDistributionResponse[];
  pieData: any;
  totalBuy =0;
  totalSell =0;
  totalClose =0;
  constructor() { }

  ngOnInit() {
    this.fillPieData();
    this.createPieChart();
  }

  fillPieData(){
    this.pieData = [];
    if (!!this.data) {
        for(var i = 0; i < this.data.length; i++){
            this.pieData.push({
                name: Util.GetRecommendationTypeDescription(this.data[i].type),
                y: this.data[i].total
            });

            if(this.data[i].type == Util.BUY)
                this.totalBuy = this.data[i].total;
            if(this.data[i].type == Util.SELL)
                this.totalSell = this.data[i].total;
            if(this.data[i].type == Util.CLOSE)
                this.totalClose = this.data[i].total;
        }
    }
  }

  createPieChart(){
    this.pieChart = new Chart({
        chart: {
          backgroundColor: null,
          plotBorderWidth: null,
          plotShadow: false,
          type: 'pie'
      },
      credits:{
        enabled: false
      },
      legend:{enabled:false},
      title: {text:null},
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
          innerSize: '70%',
          data: this.pieData
      }]
      });
  }
}
