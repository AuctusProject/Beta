import { Component, OnInit } from '@angular/core';
import { FeedResponse } from '../../../model/advisor/feedResponse';
import { AccountService } from '../../../services/account.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'advices',
  templateUrl: './advices.component.html',
  styleUrls: ['./advices.component.css']
})
export class AdvicesComponent implements OnInit {
  advices : FeedResponse[];
  hasMoreAdvices = false;
  pageSize = 10;
  promise : Subscription;
  constructor(private accountService: AccountService) { }

  ngOnInit() {
    this.loadMore();
  }

  loadMore(){
    this.promise = this.accountService.listFeed(this.pageSize, this.getLastAdviceId()).subscribe(result => 
      {
        if (this.advices == null)
          this.advices = [];
        this.advices = this.advices.concat(result);
        this.hasMoreAdvices = true;
        if(!result || result.length == 0 || result.length < this.pageSize){
          this.hasMoreAdvices = false;
        }
      }
    );
  }

  getLastAdviceId(){
    if(this.advices != null && this.advices.length > 0)
      return this.advices[this.advices.length-1].adviceId;

    return null;
  }

}
