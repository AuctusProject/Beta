import { Component, OnInit, ViewChild, Inject, PLATFORM_ID } from '@angular/core';
import { AccountService } from '../../../services/account.service';
import { NotificationsService } from 'angular2-notifications';
import { DashboardResponse } from '../../../model/admin/dashboardresponse';
import { isPlatformBrowser } from '@angular/common';
// import * as Highstock from 'highcharts/highstock';

@Component({
  selector: 'dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit {
  dashboardData: DashboardResponse = new DashboardResponse();
  @ViewChild('registrationChart') registrationChart : any;  
  @ViewChild('referralChart') referralChart: any;
  @ViewChild('advicesChart') advicesChart: any;
  @ViewChild('followingChart') followingChart : any;
  usersStartedRegistrationData: any = [];
  usersConfirmedData: any = [];
  advisorsData:  any = [];
  requestToBeAdvisorData: any = [];
  usersStartedRegistrationFlag: any = [];
  usersConfirmedFlag: any = [];
  advisorsFlag:  any = [];
  requestToBeAdvisorFlag: any = [];
  referralData: any = [];
  advicesData: any = [];
  followingData: any = [];

  constructor(private accountService : AccountService, 
    private notificationsService: NotificationsService,
    @Inject(PLATFORM_ID) private platformId: Object) { }

  ngOnInit() {
    this.accountService.getDashboard().subscribe(ret => 
    {
      this.dashboardData = ret;
      this.initiateRegistrationChart();
      this.createDonutChart(this.dashboardData.referralStatus, this.referralData, 'TOTAL REFERRALS<br><b>' + this.dashboardData.totalUsersConfirmedFromReferral + '</b>', 'Referral status', this.referralChart);
      this.createDonutChart(this.dashboardData.advices, this.advicesData, 'TOTAL ADVICES<br><b>' + this.dashboardData.totalAdvices + '</b>', 'Advices', this.advicesChart);
      this.createDonutChart(this.dashboardData.following, this.followingData, 'TOTAL FOLLOWERS<br><b>' + this.dashboardData.totalFollowing + '</b>', 'Followers', this.followingChart);
    });
  }

  fillRegistrationData(outputArray, inputArray) {
    if (inputArray) {
      for(var i = 0; i < inputArray.length; i++){
        outputArray.push(
          [Date.parse(inputArray[i].date), inputArray[i].value]
        );
      }
    }
  }

  fillFlagData(outputArray, flagInput) {
    if (flagInput) {
      outputArray.push({
          x: Date.parse(flagInput.date),
          title: flagInput.description
        });
    }
  }

  fillDistributionData(outputArray, inputArray) {
    if (inputArray) {
      for(var i = 0; i < inputArray.length; i++){
        outputArray.push(
          [inputArray[i].name, inputArray[i].amount]
        );
      }
    }
  }

  initiateRegistrationChart() {
    this.fillRegistrationData(this.usersConfirmedData, this.dashboardData.usersConfirmed);
    this.fillRegistrationData(this.usersStartedRegistrationData, this.dashboardData.usersStartedRegistration);
    this.fillRegistrationData(this.advisorsData, this.dashboardData.advisors);
    this.fillRegistrationData(this.requestToBeAdvisorData, this.dashboardData.requestToBeAdvisor);
    this.fillFlagData(this.usersConfirmedFlag, this.dashboardData.usersConfirmedLastSitutation);
    this.fillFlagData(this.usersStartedRegistrationFlag, this.dashboardData.usersStartedRegistrationLastSitutation);
    this.fillFlagData(this.advisorsFlag, this.dashboardData.advisorsLastSitutation);
    this.fillFlagData(this.requestToBeAdvisorFlag, this.dashboardData.requestToBeAdvisorLastSitutation);
    if(isPlatformBrowser(this.platformId) && window && (<any>window).Highcharts){
      (<any>window).Highcharts.chart(this.registrationChart.nativeElement, {
        rangeSelector: {
          enabled: false
        },
        scrollbar: {
          enabled: false
        },
        navigator: {
          enabled: false
        },
        credits: {
          enabled: false
        },
        chart: {
          backgroundColor: '#ffffff'
        },
        legend: {
          enabled: true,
          backgroundColor: '#ffffff',
          layout: 'horizontal',
          align: 'center',
          verticalAlign: 'bottom'
        },
        plotOptions: {
          line: {
            step: true,
            lineWidth: 4
          }
        },
        series:[
          {
            name: 'User Confirmed', 
            data: this.usersConfirmedData,
            id: 'usersConfirmedData'
          },
          {
            name: ' ', 
            color: '#ffffff',
            type: 'flags',
            data: this.usersConfirmedFlag,
            onSeries:'usersConfirmedData'
          },
          {
            name: 'User Started Registration', 
            data: this.usersStartedRegistrationData,
            id: 'usersStartedRegistrationData'
          },
          {
            name: ' ', 
            color: '#ffffff',
            type: 'flags',
            data: this.usersStartedRegistrationFlag,
            onSeries:'usersStartedRegistrationData'
          },
          {
            name: 'Experts', 
            data: this.advisorsData,
            id: 'advisorsData'
          },
          {
            name: ' ', 
            color: '#ffffff',
            type: 'flags',
            data: this.advisorsFlag,
            onSeries:'advisorsData'
          },
          {
            name: 'Request to be Expert', 
            data: this.requestToBeAdvisorData,
            id: 'requestToBeAdvisorData'
          },
          {
            name: ' ', 
            color: '#ffffff',
            type: 'flags',
            data: this.requestToBeAdvisorFlag,
            onSeries:'requestToBeAdvisorData'
          }
        ]
      });
    }
  }

  createDonutChart(inputArray, outputArray, title, seriesName, reference) {
    this.fillDistributionData(outputArray, inputArray);
    if(isPlatformBrowser(this.platformId) && window && (<any>window).Highcharts){
      return (<any>window).Highcharts.chart({
        chart: {
          renderTo:reference.nativeElement,
          plotBackgroundColor: null,
          plotBorderWidth: 0,
          plotShadow: false,
          backgroundColor: '#ffffff'
        },
        credits: {
          enabled: false
        },
        title: {
          text: title,
          align: 'center',
          verticalAlign: 'middle',
          y: 0
        },
        tooltip: {
          pointFormat: '{series.name}: <b>{point.percentage:.1f}%</b>'
        },
        plotOptions: {
          pie: {
            dataLabels: {
              enabled: true,
              distance: -50,
              style: {
                fontWeight: 'bold',
                color: 'white'
              }
            },
            startAngle: -0,
            endAngle: 0,
            center: ['50%', '50%']
          }
        },
        series: [{
            type: 'pie',
            name: seriesName,
            innerSize: '60%',
            data: outputArray
        }]
      });
    }
    return null;
  }
}
