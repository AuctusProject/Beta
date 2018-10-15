import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpService } from './http.service';
import { News } from '../model/news/news';

@Injectable()
export class NewsService {
  private getNewsUrl = this.httpService.apiUrl("v1/news/");
  constructor(private httpService : HttpService) { }

  getNews(top?: number, lastNewsId?: number): Observable<News[]> {
    var url = this.getNewsUrl + "?";
    if(!!top) url += "top=" + top;
    if (!!lastNewsId) url += "&lastNewsId=" + lastNewsId;
    return this.httpService.get(url);
  }
}
