import { Component, OnInit, OnChanges, Input, SimpleChanges } from '@angular/core';
import { DailyPerformanceResponse } from '../../../../../model/advisor/advisorPerformanceResponse';
import { CONFIG } from '../../../../../services/config.service';
import { ValueDisplayPipe } from '../../../../../util/value-display.pipe';
import { DateFormatPipe } from 'ngx-moment';

@Component({
  selector: 'daily-performance-chart',
  templateUrl: './daily-performance-chart.component.html',
  styleUrls: ['./daily-performance-chart.component.css']
})
export class DailyPerformanceChartComponent implements OnInit, OnChanges {
  @Input() dailyPerformance : DailyPerformanceResponse[];

  constructor() { }

  ngOnInit() {
  }

  ngOnChanges(changes : SimpleChanges) {
    if (changes.dailyPerformance) {
      this.lineChartData = [
        {data: this.dailyPerformance.map(d => Math.round(100 * (d.equity - CONFIG.virtualMoney)) / 100), label: 'P/L'}
      ];
  
      this.lineChartLabels = this.dailyPerformance.map(d => new Date(d.date.valueOf()));
    }  
  }

  public lineChartData:Array<any> = [];
  public lineChartLabels:Array<any> = [];
  public lineChartOptions:any = {
    responsive: true,
    maintainAspectRatio: false,
    title: {
      display: true,
      text: 'TRADER PROFIT AND LOSS'
    },
    elements: {
      line: {
          tension: 0,
          borderWidth: 2
      }
    },
    scales: {
      xAxes:[{
        type:'time', 
        time: { 
          unit: 'day'
        }
      }]
    },
    tooltips: {
      callbacks: {
        title: function(tooltipItems, data) {
            return new DateFormatPipe().transform(tooltipItems[0].xLabel, "MMM D HH:mm");
        },
          label: function(tooltipItem, data) {
              return new ValueDisplayPipe().transform(tooltipItem.yLabel);
          }
      }
    }
  };
  public lineChartColors:Array<any> = [
    { 
      backgroundColor: 'rgba(148,159,177,0.2)',
      borderColor: 'rgba(148,159,177,1)',
      pointBackgroundColor: 'rgba(148,159,177,1)',
      pointBorderColor: '#fff',
      pointHoverBackgroundColor: '#fff',
      pointHoverBorderColor: 'rgba(148,159,177,0.8)'
    }
  ];
  public lineChartLegend:boolean = false;
  public lineChartType:string = 'line';
}
