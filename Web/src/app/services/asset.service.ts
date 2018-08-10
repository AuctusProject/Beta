import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpService } from './http.service';
import { AssetResponse } from "../model/asset/assetResponse";

@Injectable()
export class AssetService {
  private baseGetAssetUrl = this.httpService.apiUrl("v1/assets/");
  
  constructor(private httpService : HttpService) { }

  getAsset(id: string): Observable<AssetResponse> {
    return this.httpService.get(this.baseGetAssetUrl + "/" + id);
  }

  getAssets(): Observable<AssetResponse[]> {
    return this.httpService.get(this.baseGetAssetUrl);
  }
}