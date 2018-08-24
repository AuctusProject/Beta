import { Component, OnInit } from '@angular/core';
import { AccountService } from '../../../services/account.service';
import { NotificationsService } from '../../../../../node_modules/angular2-notifications';
import { DashboardResponse } from '../../../model/admin/dashboardresponse';
import { StockChart } from '../../../../../node_modules/angular-highcharts';

@Component({
  selector: 'dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit {
  dashboardData: DashboardResponse = new DashboardResponse();
  registrationChart: StockChart;  
  usersStartedRegistrationData: any = [];
  usersConfirmedData: any = [];
  advisorsData:  any = [];
  requestToBeAdvisorData: any = [];
  usersStartedRegistrationFlag: any = [];
  usersConfirmedFlag: any = [];
  advisorsFlag:  any = [];
  requestToBeAdvisorFlag: any = [];

  constructor(private accountService : AccountService, private notificationsService: NotificationsService) { }

  ngOnInit() {
    this.accountService.getDashboard().subscribe(ret => 
    {
      this.dashboardData = ret;
      this.initiateRegistrationChart();
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

  initiateRegistrationChart() {
    this.fillRegistrationData(this.usersConfirmedData, this.dashboardData.usersConfirmed);
    this.fillRegistrationData(this.usersStartedRegistrationData, this.dashboardData.usersStartedRegistration);
    this.fillRegistrationData(this.advisorsData, this.dashboardData.advisors);
    this.fillRegistrationData(this.requestToBeAdvisorData, this.dashboardData.requestToBeAdvisor);
    this.fillFlagData(this.usersConfirmedFlag, this.dashboardData.usersConfirmedLastSitutation);
    this.fillFlagData(this.usersStartedRegistrationFlag, this.dashboardData.usersStartedRegistrationLastSitutation);
    this.fillFlagData(this.advisorsFlag, this.dashboardData.advisorsLastSitutation);
    this.fillFlagData(this.requestToBeAdvisorFlag, this.dashboardData.requestToBeAdvisorLastSitutation);

    this.registrationChart = new StockChart({
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
        backgroundColor: '#fafafa'
      },
      colors: ['#7cb5ec', '#fafafa', '#90ed7d', '#fafafa', '#8085e9', '#fafafa', '#e4d354', '#fafafa'],
      legend: {
        enabled: true,
        backgroundColor: '#fafafa',
        layout: 'horizontal',
        align: 'center',
        verticalAlign: 'bottom'
      },
      series:[
        {
          name: 'User Confirmed', 
          data: this.usersConfirmedData,
          id: 'usersConfirmedData'
        },
        {
          name: ' ', 
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
          type: 'flags',
          data: this.usersStartedRegistrationFlag,
          onSeries:'usersStartedRegistrationData'
        },
        {
          name: 'Advisors', 
          data: this.advisorsData,
          id: 'advisorsData'
        },
        {
          name: ' ', 
          type: 'flags',
          data: this.advisorsFlag,
          onSeries:'advisorsData'
        },
        {
          name: 'Request to be Advisor', 
          data: this.requestToBeAdvisorData,
          id: 'requestToBeAdvisorData'
        },
        {
          name: ' ', 
          type: 'flags',
          data: this.requestToBeAdvisorFlag,
          onSeries:'requestToBeAdvisorData'
        }
      ]
    }); 
  }
}
