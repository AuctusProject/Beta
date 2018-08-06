import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpService } from './http.service';
import { Asset } from "../model/asset/asset";

@Injectable()
export class AssetService {
  private baseGetAssetUrl = this.httpService.apiUrl("asset/v1");
  
  constructor(private httpService : HttpService) { }

  getAsset(id: string): Observable<Asset> {
    return this.httpService.get(this.baseGetAssetUrl + "/" + id);
  }

  getAssets(): Observable<Asset[]> {
    return this.httpService.get(this.baseGetAssetUrl);
  }
}