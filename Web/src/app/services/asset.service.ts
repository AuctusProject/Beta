import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpService } from './http.service';
import { AssetResponse } from "../model/asset/assetResponse";
import { Asset } from '../model/asset/asset';

@Injectable()
export class AssetService {
  private getAssetsDetailsUrl = this.httpService.apiUrl("v1/assets/details");
  private getAssetDetailsUrl = this.httpService.apiUrl("v1/assets/{id}/details");
  private getAssetsUrl = this.httpService.apiUrl("v1/assets/");
  private followAssetUrl = this.httpService.apiUrl("vi/assets/{id}/followers")
  constructor(private httpService : HttpService) { }

  getAssetDetails(id: string): Observable<AssetResponse> {
    return this.httpService.get(this.getAssetDetailsUrl.replace("{id}", id.toString()));
  }

  getAssetsDetails(): Observable<AssetResponse[]> {
    return this.httpService.get(this.getAssetsDetailsUrl);
  }

  getAssets(): Observable<Asset[]> {
    return this.httpService.get(this.getAssetsUrl);
  }

  followAsset(assetId:number):Observable<void>{
    return this.httpService.post(this.followAssetUrl.replace("{id}", assetId.toString()));
  }

  unfollowAsset(assetId:number):Observable<void>{
    return this.httpService.delete(this.followAssetUrl.replace("{id}", assetId.toString()));
  }
}