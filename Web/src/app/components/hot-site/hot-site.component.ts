import { Component, OnInit } from '@angular/core';
import { CONFIG } from '../../services/config.service';
import { Subscription } from 'rxjs';
import { AccountService } from '../../services/account.service';
import { EarlyAccessRequest } from '../../model/account/earlyAccessRequest';
import { NotificationsService } from 'angular2-notifications';

@Component({
  selector: 'hot-site',
  templateUrl: './hot-site.component.html',
  styleUrls: ['./hot-site.component.css']
})
export class HotSiteComponent implements OnInit {
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

  sendEarlyAccessRequest() {
    this.promise = this.accountService.postEarlyAccessRequest(this.earlyAccessRequest).subscribe(result => {
      this.earlyAccessRequest = new EarlyAccessRequest();
      this.notificationService.success(null, "You have been successfully subscribed!");
    });
  }

}
