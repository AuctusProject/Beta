import { RecommendationDistributionResponse } from "../recommendationDistributionResponse";
import { AssetResponse } from "../asset/assetResponse";

export class AdvisorResponse {
  userId: number;
  name: string;
  urlGuid: string;
  description: string;
  successRate:number;
  averageReturn:number;
  ranking:number;
  rating:number;
  numberOfFollowers?: number;
  totalAssetsAdvised: number;
  following?: boolean;
  owner: boolean;
  creationDate: Date;
  recommendationDistribution: RecommendationDistributionResponse[];
  assets?: AssetResponse[];
  
  constructor(){
    this.recommendationDistribution = [];
    this.assets = [];
  }
}

