import { RecommendationDistributionResponse } from "../recommendationDistributionResponse";

export class AssetResponse {
  assetId: number;
  name: string;
  code: string;
  totalRatings: number;
  totalAdvisors: number;
  lastValue: number;
  variation24h: number;
  variation7d: number;
  variation30d: number;
  numberOfFollowers?: number;
  following?:boolean;
  mode: number;
  recommendationDistribution: RecommendationDistributionResponse[];
  values:ValuesResponse[];
}

export class AssetAdvisorResponse
{
  userId: number;
  successRate: number;
  averageReturn: number;
  totalRatings: number;
  lastAdviceDate: Date;
  lastAdviceType: number;
  lastAdviceMode: number;
  advices: AdviceResponse[];
}

class ValuesResponse{
  date: Date;
  value: number;
}

class AdviceResponse{
  date: Date;
  adviceType: number;
}