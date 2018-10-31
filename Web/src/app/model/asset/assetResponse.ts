import { RecommendationDistributionResponse } from "../recommendationDistributionResponse";
import { AdvisorResponse } from "../advisor/advisorResponse";
import { ReportResponse } from "./reportResponse";
import { EventResponse } from "./eventResponse";

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
  pair: PairResponse;
  recommendationDistribution: RecommendationDistributionResponse[];
  reportRecommendationDistribution: RecommendationDistributionResponse[];
  assetAdvisor?: AssetAdvisorResponse[];
  advices?: AdviceResponse[];
  advisors?: AdvisorResponse[];
  reports?: ReportResponse[];
  events?: EventResponse[];
}

export class AssetAdvisorResponse
{
  userId: number;
  successRate: number;
  averageReturn: number;
  currentReturn: number;
  totalRatings: number;
  lastAdviceDate: Date;
  lastAdviceType: number;
  lastAdviceMode: number;
  lastAdviceOperationType: number;
  lastAdviceTargetPrice?: number;
  lastAdviceStopLoss?: number;
  advices: AdviceResponse[];
}

export class ValuesResponse{
  date: string;
  value: number;
}

export class AdviceResponse{
  date: string;
  adviceType: number;
  assetValue : number;
  operationType: number;
  targetPrice?: number;
  stopLoss?: number;
}

export class PairResponse{
  symbol: string;
  multipliedSymbol: string;
  preffix: string;
  suffix: string;
}