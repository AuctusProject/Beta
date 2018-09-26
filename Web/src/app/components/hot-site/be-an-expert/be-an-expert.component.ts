import { Component, OnInit } from '@angular/core';
import { Subscription } from 'rxjs';
import { CONFIG } from '../../../services/config.service';
import { AccountService } from '../../../services/account.service';
import { EarlyAccessRequest } from '../../../model/account/earlyAccessRequest';
import { NotificationsService } from 'angular2-notifications';

@Component({
  selector: 'be-an-expert',
  templateUrl: './be-an-expert.component.html',
  styleUrls: ['./be-an-expert.component.css']
})
export class BeAnExpertComponent implements OnInit {

  earlyAccessRequest: EarlyAccessRequest = new EarlyAccessRequest();
  promise: Subscription;

  constructor(private accountService: AccountService,
    private notificationService: NotificationsService) { }

  ngOnInit() {
  }

  getLogoImgUrl() {
    return CONFIG.platformImgUrl.replace("{id}", "logo_black");
  }

  getEmailInputOptions() {
    return { textOptions: { outlineField: false, placeHolder: "Email", required: false, showHintSize: false }, darkLayout:true };
  }
  
  getNameInputOptions() {
    return { textOptions: { outlineField: false, placeHolder: "Name", required: false, showHintSize: false }, darkLayout:true };
  }

  getTwitterInputOptions() {
    return { textOptions: { outlineField: false, placeHolder: "Twitter", required: false, showHintSize: false }, darkLayout:true };
  }

  sendEarlyAccessRequest() {
    this.promise = this.accountService.postEarlyAccessRequest(this.earlyAccessRequest).subscribe(result => {
      this.earlyAccessRequest = new EarlyAccessRequest();
      this.notificationService.success(null, "You have been successfully subscribed!");
    });
  }
}
