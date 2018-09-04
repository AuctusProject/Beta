import { RecommendationDistributionResponse } from "../recommendationDistributionResponse";
import { AdvisorResponse } from "../advisor/advisorResponse";

export class AssetResponse {
  assetId: number;
  name: string;
  code: string;
  totalRatings?: number;
  totalAdvisors?: number;
  lastValue?: number;
  variation24h?: number;
  variation7d?: number;
  variation30d?: number;
  numberOfFollowers?: number;
  following?:boolean;
  mode: number;
  recommendationDistribution: RecommendationDistributionResponse[];
  values?:ValuesResponse[];
  assetAdvisor?: AssetAdvisorResponse[];
  advices?: AdviceResponse[];
  advisors?: AdvisorResponse[];
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

export class ValuesResponse{
  date: string;
  value: number;
}

export class AdviceResponse{
  date: string;
  adviceType: number;
}