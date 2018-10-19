import {Inject, Injectable} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import { Observable } from 'rxjs';
import { catchError } from 'rxjs/operators';

export class Config
{
  readonly apiUrl: string;
  readonly webUrl: string;
  readonly reportUrl: string;
  readonly eventUrl: string;
  readonly agencyImgUrl: string;
  readonly assetImgUrl: string;
  readonly profileImgUrl: string;
  readonly platformImgUrl: string;
}

export let CONFIG: Config;

@Injectable()
export class ConfigService
{

  constructor(private http: HttpClient)
  { }

  public load()
  {
    var t = new Config();
    CONFIG = Object.assign(t, {
        apiUrl: "http://localhost:50427/api/",
        webUrl: "http://localhost:4200",
        reportUrl: "https://auctusplatform.azureedge.net/assetsreport/{id}.pdf",
        eventUrl: "https://auctusplatform.azureedge.net/assetsevent/{id}.png",
        agencyImgUrl: "https://auctusplatform.azureedge.net/agencieslogo/{id}.png",
        assetImgUrl: "https://auctusplatform.azureedge.net/assetsicons/{id}.png",
        profileImgUrl: "https://auctusplatform.azureedge.net/userpicture/{id}.png",
        platformImgUrl: "https://auctusplatform.azureedge.net/platform/{id}.png"
    });
    // return new Promise((resolve, reject) => {
    //   this.http.get('/assets/config/config.json').pipe(
    //     catchError((error: any): any => {
    //     reject(true);
    //     //return Observable.throw('Server error');
    //   })
      
    // ).subscribe((envResponse :any) => {
    //   let t = new Config();
    //   //Modify envResponse here if needed (e.g. to ajust parameters for https,...)
    //   CONFIG = Object.assign(t, envResponse);
    //   resolve(true);
    // });
    // });
  }
}