export class AssetRatingsResponse {
  assetId :number;
  expertId:number;
  expertName:string;
  expertRating:number;
  adviceDate?:Date;
  adviceType?:number;
  assetValue?:number;
  stopLoss?:number;
  targetPrice?:number;
}