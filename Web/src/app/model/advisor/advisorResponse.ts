import { RecommendationDistributionResponse } from "../recommendationDistributionResponse";
import { PairResponse } from "../asset/assetResponse";

export class AdvisorResponse {
  userId: number;
  name: string;
  urlGuid: string;
  description: string;
  successRate: number;
  averageReturn: number;
  ranking: number;
  totalAdvisors: number;
  rating: number;
  numberOfFollowers?: number;
  totalAssetsTraded: number;
  totalTrades: number;
  following?: boolean;
  owner: boolean;
  creationDate: Date;
  totalAvailable: number;
  totalAllocated: number;
  totalBalance: number;
  equity: number;
  totalProfit: number;
  totalProfitPercentage: number;
  averageTradeMinutes?: number;
  lastPortfolioReferenceDate?: Date;
  lastPortfolioValue?: number;
  profit24hValue?: number;
  profit24hPercentage?: number;
  monthlyRankingHistory: MonthlyRankingHistoryResponse;
  recommendationDistribution: RecommendationDistributionResponse[];
  openPositions?: AdvisorAssetResponse[];
  closedPositions?: AdvisorAssetResponse[];
  advisorAsset24hHistory?: AdvisorAssetHistoryResponse[];
  
  constructor(){
    this.recommendationDistribution = [];
    this.openPositions = [];
    this.closedPositions = [];
    this.advisorAsset24hHistory = [];
  }
}

export class AdvisorAssetResponse {
  assetId: number;
  assetName: string;
  assetCode: string;
  pair: PairResponse;
  type: number;
  successRate: number;
  averageReturn: number;
  averagePrice: number;
  totalQuantity: number;
  totalInvested: number;
  totalProfit: number;
  totalVirtual: number;
  orderCount: number;
  successCount: number;
  summedTradeMinutes?: number;
}

export class AdvisorAssetHistoryResponse {
  assetId: number;
  averagePrice: number;
  totalQuantity: number;
  totalInvested: number;
  totalProfit: number;
  totalVirtual: number;
  profit24hValue?: number;
  profit24hPercentage?: number;
}

export class PositionResponse {
  totalInvested: number;
  totalProfit: number;
  totalVirtual: number;
  totalQuantity: number;
  averageReturn: number;
  successRate: number;
  averagePrice: number;
  orderCount: number;
  successCount: number;
  averageTradeMinutes?: number;
  type?: number;
}

export class AssetPositionResponse {
  assetId: number;
  assetCode: string;
  assetName: string;
  positionResponse: PositionResponse;
}

export class MonthlyRankingHistoryResponse {
  ranking: number;
  portfolioValue?: number;
  portfolioReferenceDate?: Date;
  profitPercentage: number;
}