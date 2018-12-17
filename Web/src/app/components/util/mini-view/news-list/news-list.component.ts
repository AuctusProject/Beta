import { Component, OnInit, NgZone, ChangeDetectorRef } from '@angular/core';
import { NewsService } from '../../../../services/news.service';
import { News } from '../../../../model/news/news';
import { DatePipe } from '@angular/common';
import { TimeAgoPipe } from 'ngx-moment';
import { EventsService } from 'angular-event-service';

 @Component({
  selector: 'news-list',
  templateUrl: './news-list.component.html',
  styleUrls: ['./news-list.component.css']
})
export class NewsListComponent implements OnInit {
  hasMoreNews = false;
  pageSize = 20;
  news: News[];
  displayedColumns: string[] = ['source', 'title'];
   constructor(private newsService: NewsService,
    private zone : NgZone,
    private changeDetectorRef: ChangeDetectorRef,
    private eventsService: EventsService) { }
   ngOnInit() {
    this.loadNews();
    this.eventsService.on("addLastNews", (data) => this.onDataReceive(data, this));
  }
  
   onDataReceive(data:News[], _this){
    for(var news of data){
      news.signalR = true;
    }
    _this.news = data.concat(_this.news);
    setTimeout(()=> {for(var news of data){
      news.signalR = false;
    }},30000);
  }
   loadNews(){
    this.newsService.getNews(this.pageSize, this.getLastNewsId()).subscribe(result => {
      if(this.news == null){
        this.news = [];
      }
      this.news = this.news.concat(result);
      this.hasMoreNews = true;
      if(!result || result.length == 0 || result.length < this.pageSize){
        this.hasMoreNews = false;
      }
    });
  }
   getLastNewsId() {
    if(this.news != null && this.news.length > 0){
      return this.news[this.news.length-1].id;
    }
    return null;
  }
   onScroll() {
    this.loadNews();
  }
  ONE_DAY:number = 24 * 60 * 60 * 1000;
  getNewsTime(date){
    if((new Date().getTime()) - new Date(date).getTime() < this.ONE_DAY){
      return new DatePipe("en-US").transform(date, "mediumTime");
    }
    else{
      return new TimeAgoPipe(this.changeDetectorRef, this.zone).transform(date);
    }
  }
}