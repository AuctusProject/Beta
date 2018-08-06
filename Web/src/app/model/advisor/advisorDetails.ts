import { RecommendationDistribution } from "../recommendationDistribution";

export class AdvisorDetails {
  successRate: number;
  averageReturn: number;
  ranking: number;
  numberOfFollowers: number;
  recommendationDistribution: RecommendationDistribution[];
  following: boolean;
  rating: number;

  constructor(){
    this.recommendationDistribution = [];
  }
}
