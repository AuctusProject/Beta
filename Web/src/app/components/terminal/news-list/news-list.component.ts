import { Component, OnInit, NgZone } from '@angular/core';
import { NewsService } from 'src/app/services/news.service';
import { News } from 'src/app/model/news/news';
import { HubConnectionBuilder, HubConnection } from '@aspnet/signalr';
import { CONFIG } from 'src/app/services/config.service';

@Component({
  selector: 'news-list',
  templateUrl: './news-list.component.html',
  styleUrls: ['./news-list.component.css']
})
export class NewsListComponent implements OnInit {
  hasMoreNews = false;
  pageSize = 20;
  news: News[];
  connection: HubConnection;
  displayedColumns: string[] = ['date', 'source', 'title'];

  constructor(private newsService: NewsService,
    private zone : NgZone) { }

  ngOnInit() {
    this.loadNews();
    this.connection = new HubConnectionBuilder()
    .withUrl(CONFIG.apiUrl + "auctusHub")
    .build();
    this.connection.onclose(() => this.startConnection(this));
    this.connection.on("addLastNews", (data) => this.onDataReceive(data, this));
    this.startConnection(this);
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
}
