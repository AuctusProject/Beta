import { Component, OnInit } from '@angular/core';
import { AccountService } from '../../../services/account.service';
import { AssetResponse } from '../../../model/asset/assetResponse';
import { AdvisorResponse } from '../../../model/advisor/advisorResponse';
import { LoginResponse } from '../../../model/account/loginResponse';

@Component({
  selector: 'watchlist',
  templateUrl: './watchlist.component.html',
  styleUrls: ['./watchlist.component.css']
})
export class WatchlistComponent implements OnInit {
  assets : AssetResponse[];
  experts : AdvisorResponse[];
  loginData: LoginResponse;

  constructor(private accountService: AccountService) { }

  ngOnInit() {
    this.loginData = this.accountService.getLoginData();
    if (this.loginData) {
      this.accountService.getAssetsFollowedByUser(this.loginData.id).subscribe(result => 
        {
          this.assets = result;
        });

        this.accountService.getExpertsFollowedByUser(this.loginData.id).subscribe(result => 
          {
            this.experts = result;
          });
      }
  }

}
