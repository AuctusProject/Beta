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

  constructor() { }

  ngOnInit() {
    this.fillPieData();
    this.createPieChart();
  }

  fillPieData(){
    this.pieData = [];
    for(var i =0;i<this.data.length;i++){
        this.pieData.push({
            name: Util.GetRecommendationTypeDescription(this.data[i].type),
            y: this.data[i].total
        });
    }
  }

  createPieChart(){
    this.pieChart = new Chart({
        chart: {
          plotBackgroundColor: null,
          plotBorderWidth: null,
          plotShadow: false,
          type: 'pie'
      },
      credits:{
        enabled: false
      },
      legend:{
        labelFormat: '{name}: {percentage:.1f}%'
      },
      title: {
          text: 'Recommendation Distribution'
      },
      tooltip: {
          pointFormat: '<b>{point.percentage:.1f}%</b>'
      },
      plotOptions: {
          pie: {
              allowPointSelect: true,
              cursor: 'pointer',
              dataLabels: {
                  enabled: false,
                  format: '<b>{point.name}</b>: {point.percentage:.1f}%',
                  style: {
                      color: 'black'
                  }
              },
              showInLegend: true
          }
      },
      series: [{
          name: 'Recommendations',
          data: this.pieData
      }]
      });
  }
}
