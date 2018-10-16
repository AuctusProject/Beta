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
  pageSize = 20;
  news: News[];
  displayedColumns: string[] = ['date', 'source', 'title'];

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

    connection.on("addLastNews", this.onDataReceive);
  }

  onDataReceive(data:News[]){
    for(var news of data){
      news.signalR = true;
    }
    this.news = data.concat(this.news);
    setTimeout(()=> {for(var news of data){
      news.signalR = false;
    }},30000);
  }

  addNews(){
    var data : News[] = [
      {
        id:130,
        title:"What is Bitcoin’s Liquid sidechain and why does it matter?",
        externalCreationDate: new Date(),
        creationDate: new Date(),
        link:"https://cryptoinsider.com/myTest",
        newsSource:{"id":5,"name":"Crypto Insider","url":"https://cryptoinsider.com/feed"},
        signalR: false
      }
      ];
      this.onDataReceive(data);
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
