export class RecommendationDistribution {
  type:number;
  total:number;

  constructor(type?:number, total?:number){
    this.type=type;
    this.total=total;    
  }

  getTypeDescription(){
    if(this.type == 0){
      return "SELL";
    }
    else if(this.type == 1){
      return "BUY";
    }
    else{
      return "CLOSE";
    }
  }
}
