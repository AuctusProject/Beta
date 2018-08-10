export class Util {
    public static DiffDays(date1, date2) : number {
        var oneDay = 24 * 60 * 60 * 1000;
        var diffDays = Math.round(Math.abs((date1.getTime() - date2.getTime()) / (oneDay)));
        return diffDays;
    }

    public static Sort<T>(list: T[],  prop: (c: T) => any, order?: "ASC" | "DESC"): void {
      list.sort((a, b) => {
          if (prop(a) < prop(b))
              return -1;
          if (prop(a) > prop(b))
              return 1;
          return 0;
      });
  
      if (order === "DESC") {
          list.reverse();
      }        
    }

    public static GetRecommendationTypeDescription(type: number){
        if(type == 0){
            return "SELL";
          }
          else if(type == 1){
            return "BUY";
          }
          else{
            return "CLOSE";
          }
    }
  }