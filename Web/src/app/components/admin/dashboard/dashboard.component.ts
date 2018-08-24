import { Component, OnInit } from '@angular/core';
import { AccountService } from '../../../services/account.service';
import { NotificationsService } from '../../../../../node_modules/angular2-notifications';
import { DashboardResponse } from '../../../model/admin/dashboardresponse';

@Component({
  selector: 'dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit {
  dashboardData: DashboardResponse = new DashboardResponse();

  constructor(private accountService : AccountService, private notificationsService: NotificationsService) { }

  ngOnInit() {
    this.accountService.getDashboard().subscribe(ret => 
    {
      this.dashboardData = ret;
    });
  }
}
