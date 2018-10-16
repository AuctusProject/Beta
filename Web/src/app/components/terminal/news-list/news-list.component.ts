import { Component, OnInit } from '@angular/core';
import { NewsService } from 'src/app/services/news.service';
import { News } from 'src/app/model/news/news';
import { HubConnectionBuilder } from '@aspnet/signalr';
import { CONFIG } from 'src/app/services/config.service';

@Component({
  selector: 'news-list',
  templateUrl: './news-list.component.html',
  styleUrls: ['./news-list.component.css']
})
export class NewsListComponent implements OnInit {
  hasMoreNews = false;
  pageSize = 10;
  news: News[] = [];
  displayedColumns: string[] = ['title', 'date', 'added'];

  constructor(private newsService: NewsService) { }

  ngOnInit() {
    this.loadNews();
    let connection = new HubConnectionBuilder()
    .withUrl(CONFIG.apiUrl + "auctusHub")
    .build();
    connection
      .start()
      .then(() => console.log('Connection started!'))
      .catch(err => console.log('Error while establishing connection :('));

    connection.on("addLastNews", data => {
        console.log(data);
        this.news = data.concat(this.news);
    });
  }

  loadNews(){
    this.newsService.getNews(this.pageSize, this.getLastNewsId()).subscribe(result => {
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
