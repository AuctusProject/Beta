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
  constructor(private accountService: AccountService) { }

  ngOnInit() {
    this.appendFeed();
  }

  appendFeed(){
    this.accountService.listFeed(10, this.getLastAdviceId()).subscribe(result => 
      this.advices = this.advices.concat(result));
  }

  getLastAdviceId(){
    if(this.advices.length > 0)
      return this.advices[this.advices.length-1].adviceId;

    return null;
  }

}
