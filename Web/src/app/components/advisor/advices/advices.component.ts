import { Component, OnInit } from '@angular/core';
import { FeedResponse } from '../../../model/advisor/feedResponse';
import { AccountService } from '../../../services/account.service';

@Component({
  selector: 'advices',
  templateUrl: './advices.component.html',
  styleUrls: ['./advices.component.css']
})
export class AdvicesComponent implements OnInit {
  advices : FeedResponse[] = [];
  hasMoreAdvices = true;
  pageSize = 10;
  constructor(private accountService: AccountService) { }

  ngOnInit() {
    this.loadMore();
  }

  loadMore(){
    this.accountService.listFeed(this.pageSize, this.getLastAdviceId()).subscribe(result => 
      {
        this.advices = this.advices.concat(result);
        if(!result || result.length == 0 || result.length < this.pageSize){
          this.hasMoreAdvices = false;
        }
      }
    );
  }

  getLastAdviceId(){
    if(this.advices.length > 0)
      return this.advices[this.advices.length-1].adviceId;

    return null;
  }

}
