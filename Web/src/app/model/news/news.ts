export class News {
    id: number;
    title: string;
    externalCreationDate:Date;
    creationDate: Date;
    link:string;
    newsSource: NewsSource;
    signalR:boolean;
  }

export class NewsSource{
  id:number;
  name: string;
  url: string;
}