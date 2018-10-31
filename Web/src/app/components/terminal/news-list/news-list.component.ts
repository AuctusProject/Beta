import { Component, OnInit, NgZone, ChangeDetectorRef, Inject, PLATFORM_ID, OnDestroy } from '@angular/core';
import { NewsService } from '../../../services/news.service';
import { News } from '../../../model/news/news';
import { HubConnectionBuilder, HubConnection } from '@aspnet/signalr';
import { CONFIG } from '../../../services/config.service';
import { DatePipe, isPlatformBrowser } from '@angular/common';
import { TimeAgoPipe } from 'time-ago-pipe';

@Component({
  selector: 'news-list',
  templateUrl: './news-list.component.html',
  styleUrls: ['./news-list.component.css']
})
export class NewsListComponent implements OnInit, OnDestroy {
  hasMoreNews = false;
  pageSize = 20;
  news: News[];
  connection: HubConnection;
  displayedColumns: string[] = ['date', 'source', 'title'];

  constructor(private newsService: NewsService,
    private zone : NgZone,
    private changeDetectorRef: ChangeDetectorRef,
    @Inject(PLATFORM_ID) private platformId: Object) { }

  ngOnInit() {
    this.loadNews();
    if(isPlatformBrowser(this.platformId)){
      this.connection = new HubConnectionBuilder()
      .withUrl(CONFIG.apiUrl + "auctusHub")
      .build();
      this.connection.onclose(() => this.startConnection(this));
      this.connection.on("addLastNews", (data) => this.onDataReceive(data, this));
      this.startConnection(this);
    }
  }

  ngOnDestroy(){
    if(isPlatformBrowser(this.platformId)){
      this.connection.off("close");
      this.connection.stop();
    }
  }

  startConnection(_this){
    this.connection
    .start()
    .then(() => console.log('Connection started!'))
    .catch(err => {
      console.log('Error while establishing connection :('); 
      setTimeout(() => _this.zone.run(() => _this.startConnection(_this), 5000));
    });
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
