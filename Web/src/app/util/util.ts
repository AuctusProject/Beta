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
        if(type == this.SELL){
            return "SELL";
          }
          else if(type == this.BUY){
            return "BUY";
          }
          else{
            return "CLOSE";
          }
    }

    public static GetRecommendationTypeColor(type: number){
        if(type == this.SELL){
            return "#d13e3e";
        }
        else if(type == this.BUY){
            return "#3ed142";
        }
        else{
            return "#999999";
        }
    }

    public static GetGeneralRecommendationDescription(type: number){
        if(type == this.AssetModeType.Neutral){
            return "Neutral";
        }
        else if(type == this.AssetModeType.ModerateBuy){
            return "Moderate Buy";
        }
        else if(type == this.AssetModeType.StrongBuy){
            return "Strong Buy";
        }
        else if(type == this.AssetModeType.ModerateSell){
            return "Moderate Sell";
        }
        else if(type == this.AssetModeType.StrongSell){
            return "Strong Sell";
        }
        else if(type == this.AssetModeType.Close){
            return "Close";
        }
    }

    public static GetAdviceModeDescription(type: number){
        if(type == this.AdviceModeType.Initiate){
            return "Initiate";
        }
        else if(type == this.AdviceModeType.Reiterate){
            return "Reiterate";
        }
        else if(type == this.AdviceModeType.Upgrade){
            return "Upgrade";
        }
        else if(type == this.AdviceModeType.Downgrade){
            return "Downgrade";
        }
    }

    public static SELL: number = 0;
    public static BUY: number = 1;
    public static CLOSE: number = 2;

    static AssetModeType = class{
        public static Neutral = 0;
        public static ModerateBuy = 1;
        public static StrongBuy = 2;
        public static ModerateSell = 3;
        public static StrongSell = 4;
        public static Close = 5;
    }

    static AdviceModeType = class{
        public static Initiate = 0;
        public static Reiterate = 1;
        public static Upgrade = 2;
        public static Downgrade = 3;
    }
}