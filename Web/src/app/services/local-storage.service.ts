import { Injectable, PLATFORM_ID, Inject } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';


@Injectable()
export class LocalStorageService {

  constructor(@Inject(PLATFORM_ID) private platformId: Object) { }

  public setLocalStorage(key: string, value: any): void {
    if (isPlatformBrowser(this.platformId)) if (window) window.localStorage.setItem(key, typeof value === "string" ? value : JSON.stringify(value));
  }

  public getLocalStorage(key: string): any {
    return isPlatformBrowser(this.platformId) && window ? window.localStorage.getItem(key) : null;
  }

  public removeLocalStorage(key: string): void {
    if (isPlatformBrowser(this.platformId)) if (window) window.localStorage.removeItem(key);
  }

}
