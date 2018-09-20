import { Injectable } from '@angular/core';
import { LocalStorageService } from './local-storage.service';
import { Observable } from 'rxjs';


@Injectable()
export class LocalCacheService {
  defaultMaxCacheMinutes = 10;
  constructor(private localStorageService : LocalStorageService) { }

  
  public requestWithCache(req : Observable<any>, key: string, maxCacheMinutes?:number) : Observable<any>{
    var cached : CacheStructure = JSON.parse(this.localStorageService.getLocalStorage(key));
    if(!cached || !cached.object || this.isExpired(cached.cacheMilliseconds, maxCacheMinutes)){
      return new Observable(observer => 
        {
          req.subscribe(result => {
            this.localStorageService.setLocalStorage(key, new CacheStructure(result));
            observer.next(result);
          })
        });
    }
    else{
      return new Observable(observer => observer.next(cached.object))
    }
  }

  private isExpired(cacheMilliseconds:number, maxCacheMinutes?:number):boolean{
    return cacheMilliseconds < (Date.now() - 
      ((maxCacheMinutes != null ? maxCacheMinutes : this.defaultMaxCacheMinutes) * 60 * 1000));
  }
}

export class CacheStructure{
  cacheMilliseconds: number;
  object: any;

  constructor(object: any){
      this.cacheMilliseconds = Date.now();
      this.object = object;
    }
}
