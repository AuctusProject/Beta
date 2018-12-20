import { Constants } from "./constants";
import { CONFIG } from "../services/config.service";

export class Util {
    public static SELL: number = 0;
    public static BUY: number = 1;
    public static CLOSE: number = 2;

    public static DiffDays(date1, date2) : number {
        var oneDay = 24 * 60 * 60 * 1000;
        var diffDays = Math.round(Math.abs((date1.getTime() - date2.getTime()) / (oneDay)));
        return diffDays;
    }

    public static ConvertUTCDateStringToLocalDate(date) {
        return this.ConvertUTCDateToLocalDate(new Date(date));
    }

    public static GetProfit(baseOrderType: number, basePrice: number, baseFee: number, baseQuantity: number, quantity: number, currentPrice: number) {
        var closedDollar = this.GetClosedDollar(baseOrderType, basePrice, quantity, currentPrice);
        var baseFeePercentage = this.GetBaseFeePercentage(basePrice, baseFee, baseQuantity);
        return this.GetProfitValue(closedDollar, basePrice, quantity, baseFeePercentage);
    }

    public static GetProfitValue(closedDollar: number, basePrice: number, quantity: number, baseFeePercentage: number) {
        return closedDollar / (basePrice * quantity / (1 - baseFeePercentage)) - 1;
    }

    public static GetBaseFeePercentage(basePrice: number, baseFee: number, baseQuantity: number) {
        return baseFee / (baseQuantity * basePrice + baseFee);
    }

    public static GetClosedDollar(baseOrderType: number, basePrice: number, quantity: number, currentPrice: number) {
        var expectedCloseValue = this.GetExpectedCloseValue(baseOrderType, basePrice, quantity, currentPrice);
        var fee = expectedCloseValue * CONFIG.orderFee;
        return expectedCloseValue - fee;
    }

    public static GetExpectedCloseValue(baseOrderType: number, basePrice: number, quantity: number, currentPrice: number) {
        return (baseOrderType == Constants.OrderType.Buy ? currentPrice * quantity : quantity * (2 * basePrice - currentPrice));
    }

    public static ConvertUTCDateToLocalDate(date) {
        var newDate = new Date(date.getTime()+date.getTimezoneOffset()*60*1000);
    
        var offset = date.getTimezoneOffset() / 60;
        var hours = date.getHours();
    
        newDate.setHours(hours - offset);
    
        return newDate;   
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
            return "#383838";
        }
    }

    public static GetOrderTypeText(type: number) {
        return type == 0 ? "Sell" : "Buy";
    }

    public static CustomSortingData(data: any, sortHeaderId: string){
        var property = sortHeaderId.split('.').reduce((o, p) => o && o[p], data)

        if (typeof property === 'string') {
          return property.toLocaleLowerCase();
        }
      
        return property;
    }

    public static GetNumberWithOrdinalSuffix(i) {
        var j = i % 10,
            k = i % 100;
        if (j == 1 && k != 11) {
            return i + "st";
        }
        if (j == 2 && k != 12) {
            return i + "nd";
        }
        if (j == 3 && k != 13) {
            return i + "rd";
        }
        return i + "th";
    }

    public static GetMonthName(monthNumber) {
        var monthNames = [ 'January', 'February', 'March', 'April', 'May', 'June',
            'July', 'August', 'September', 'October', 'November', 'December' ];
        return monthNames[monthNumber - 1];
    }
}