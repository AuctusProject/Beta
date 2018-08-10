export class RecommendationDistributionResponse {
  type:number;
  total:number;

  constructor(type?:number, total?:number){
    this.type=type;
    this.total=total;    
  }
}
