import { Component, OnInit } from '@angular/core';
import { AccountService } from '../../../services/account.service';
import { NotificationsService } from 'angular2-notifications';
import { ConfigurationRequest } from '../../../model/account/configurationRequest';

@Component({
  selector: 'configuration',
  templateUrl: './configuration.component.html',
  styleUrls: ['./configuration.component.css']
})
export class ConfigurationComponent implements OnInit {

  configurationRequest: ConfigurationRequest = new ConfigurationRequest();
  wallet: string;

  constructor(private accountService : AccountService, private notificationsService: NotificationsService) { }

  ngOnInit() {
    this.accountService.getConfiguration().subscribe(result =>
      {
        this.wallet = result.wallet;
        this.configurationRequest.allowNotifications = result.allowNotifications;
      }
    );
  }

  save(){
    this.accountService.setConfiguration(this.configurationRequest).subscribe(result =>
      this.notificationsService.success(null, "Successfully saved.")
    );
  }
}
