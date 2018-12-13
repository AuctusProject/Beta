import {Inject, Injectable, PLATFORM_ID} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import { Observable } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { isPlatformBrowser } from '@angular/common';

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
  readonly virtualMoney: number;
}

export let CONFIG: Config;

@Injectable()
export class ConfigService
{

  constructor(private http: HttpClient, @Inject(PLATFORM_ID) private platformId: Object)
  {
  }

  public load()
  {
    if(!isPlatformBrowser(this.platformId)){
      var t = new Config();
      CONFIG = Object.assign(t, {
        apiUrl: "https://auctusplatformapi.azurewebsites.net/api/",
        webUrl: "https://experts.auctus.org",
        reportUrl: "https://auctus.azureedge.net/assetsreport/{id}.pdf",
        eventUrl: "https://auctus.azureedge.net/assetsevent/{id}.png",
        agencyImgUrl: "https://auctus.azureedge.net/agencieslogo/{id}.png",
        assetImgUrl: "https://auctus.azureedge.net/assetsicons/{id}.png",
        profileImgUrl: "https://auctus.azureedge.net/userpicture/{id}.png",
        platformImgUrl: "https://auctus.azureedge.net/platform/{id}.png",
        virtualMoney: 100000
      });
    }
    else{
      return new Promise((resolve, reject) => {
        this.http.get('assets/config/config.json').pipe(
          catchError((error: any): any => {
          reject(true);
          return Observable.throw('Server error');
        })
        
      ).subscribe((envResponse :any) => {
        let t = new Config();
        CONFIG = Object.assign(t, envResponse);
        resolve(true);
      });
      });
    }
  }
}