import {Inject, Injectable} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import { Observable } from 'rxjs';
import { catchError } from 'rxjs/operators';

export class Config
{
  readonly apiUrl: string;
  readonly assetImgUrl:string;
  readonly profileImgUrl:string;
}

export let CONFIG: Config;

@Injectable()
export class ConfigService
{

  constructor(private http: HttpClient)
  {
  }

  public load()
  {
    return new Promise((resolve, reject) => {
      this.http.get('/assets/config/config.json').pipe(
        catchError((error: any): any => {
        reject(true);
        return Observable.throw('Server error');
      })
      
    ).subscribe((envResponse :any) => {
      let t = new Config();
      //Modify envResponse here if needed (e.g. to ajust parameters for https,...)
      CONFIG = Object.assign(t, envResponse);
      resolve(true);
    });
    });
  }
}