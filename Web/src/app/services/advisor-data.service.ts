import { Injectable } from "@angular/core";
import { empty, Subject, Observable, Subscription } from "rxjs";
import { TickerService } from "./ticker.service";
import { AdvisorResponse, PositionResponse, AssetPositionResponse } from "../model/advisor/advisorResponse";
import { AdvisorService } from "./advisor.service";
import { PairResponse } from "../model/asset/assetResponse";
import { CONFIG } from "./config.service";
import { Constants } from "../util/constants";
import { Util } from "../util/util";

@Injectable()
export class AdvisorDataService {
  private advisorData:  AdvisorResponse;
  private advisorListData:  AdvisorResponse[];
  private closePosition: { [advisorId: number]: PositionResponse; } = {};
  private closeAssetPosition: { [advisorId: number]: AssetPositionResponse[] } = {};
  private openAssetsId: number[];
  private assetData: { [assetId: number]: {name, code}; } = {};
  private advisorId: number;
  private initialized: boolean = false;
  private advisorListResponseSubject = new Subject<AdvisorResponse[]>(); 
  private advisorResponseSubject = new Subject<AdvisorResponse>(); 
  private openPositionResponseSubject = new Subject<PositionResponse>(); 
  private closePositionResponseSubject = new Subject<PositionResponse>(); 
  private openPositionAssetResponseSubject: { [assetId: number]: Subject<AssetPositionResponse>; } = {};
  private allAssetsOpenPositionAssetResponseSubject = new Subject<AssetPositionResponse[]>();
  private allAssetsClosePositionAssetResponseSubject = new Subject<AssetPositionResponse[]>();
  private binanceTickers: Subscription[] = [];
  private pairPrice: { [pair: string]: { [type: number]: number }; } = {};
  private openPositionAssetResponse: { [advisorId: number]: { [assetId: number]: PositionResponse[] } } = {};

  public constructor(private advisorService : AdvisorService, private tickerService: TickerService) { }
    
  public initialize(advisorId?: number) : void {
    if (!this.initialized) {
      this.advisorId = advisorId;
      this.initialized = true;
      this.refresh();
    }
  }

  public initializeWithListResponse(advisorListResponse: AdvisorResponse[]) : void {
    if (advisorListResponse) {
      this.advisorListData = advisorListResponse;
      this.initialized = true;
      this.openAssetsId = [];
      this.assetData = {};
      let openAssetsPair = this.setOpenAssetsData(this.advisorListData);
      for (let i = 0; i < this.advisorListData.length; ++i) {
        this.setClosePositions(this.advisorListData[i]);
        this.refreshAllData(this.advisorListData[i]);
      }
      for (let i = 0; i < openAssetsPair.length; ++i) {
        this.setBinanceTicker(openAssetsPair[i]);
      }
    }
  }

  public concatAdvisorListResponse(advisorListResponse: AdvisorResponse[]) : void {
    if (this.initialized) {
      let openAssetsPair = this.setOpenAssetsData(advisorListResponse);
      for (let i = 0; i < advisorListResponse.length; ++i) {
        this.setClosePositions(advisorListResponse[i]);
        this.refreshAllData(advisorListResponse[i]);
      }
      for (let i = 0; i < openAssetsPair.length; ++i) {
        this.setBinanceTicker(openAssetsPair[i]);
      }
      this.advisorListData = this.advisorListData.concat(advisorListResponse);
    }
  }

  public destroy() : void {
    if (this.binanceTickers) {
      for (let i = 0; i < this.binanceTickers.length; ++i) {
        this.binanceTickers[i].unsubscribe();
      }
    }
    this.initialized = false;
    this.openAssetsId = [];
    this.binanceTickers = [];
  }

  public refresh() : void {
    if (this.advisorId) {
      this.advisorService.getExpertDetails(this.advisorId).subscribe(ret => this.setAdvisorResponse(ret));
    } else if (!this.advisorListData) {
      this.advisorService.getLoggedExpertDetails().subscribe(ret => this.setAdvisorResponse(ret));
    }
  }

  public advisorResponse() : Observable<AdvisorResponse> {
    return this.advisorResponseSubject.asObservable();
  }

  public advisorListResponse() : Observable<AdvisorResponse[]> {
    return this.advisorListResponseSubject.asObservable();
  }

  public openPositions() : Observable<PositionResponse> {
    return this.openPositionResponseSubject.asObservable();
  }

  public listAllOpenPositions() : Observable<AssetPositionResponse[]> {
    return this.allAssetsOpenPositionAssetResponseSubject.asObservable();
  }

  public listAllClosePositions() : Observable<AssetPositionResponse[]> {
    return this.allAssetsClosePositionAssetResponseSubject.asObservable();
  }

  public closePositions() : Observable<PositionResponse> {
    return this.closePositionResponseSubject.asObservable();
  }

  public openPosition(assetId: number) : Observable<AssetPositionResponse> {
    if (!this.openPositionAssetResponseSubject[assetId]) {
      this.openPositionAssetResponseSubject[assetId] = new Subject<AssetPositionResponse>();
    } 
    return this.openPositionAssetResponseSubject[assetId].asObservable();
  }

  public getOpenPositionAssetsId() : number[] {
    return this.openAssetsId;
  }

  private setAdvisorResponse(response: AdvisorResponse) : void {
    let oldOpenAssetsId = null;
    if (this.advisorData && response && this.advisorData.userId == response.userId && this.openAssetsId 
      && this.openAssetsId.length > 0) {
      oldOpenAssetsId = this.openAssetsId.slice(0);
    }
    this.destroy();
    this.advisorData = response;
    this.openAssetsId = [];
    this.assetData = {};
    let openAssetsPair = this.setOpenAssetsData([this.advisorData]);
    if (oldOpenAssetsId) {
      for (let i = 0; i < oldOpenAssetsId.length; ++i) {
        let found = false;
        for (let j = 0; j < this.openAssetsId.length; ++j) {
          if (oldOpenAssetsId[i] == this.openAssetsId[j]) {
            found = true;
            break;
          }
        }
        if (!found) {
          this.openPositionAssetResponseSubject[oldOpenAssetsId[i]].next(null);
        }
      }
    }
    this.setClosePositions(this.advisorData);
    this.refreshAllData(this.advisorData);
    for (let i = 0; i < openAssetsPair.length; ++i) {
      this.setBinanceTicker(openAssetsPair[i]);
    }
  }

  private setOpenAssetsData(responses: AdvisorResponse[]) : PairResponse[] {
    let openAssetsPair = [];
    if (responses) {
      for (let k = 0; k < responses.length; ++k) {
        if (responses[k].openPositions) {
          for (let i = 0; i < responses[k].openPositions.length; ++i) {
            let openPosition = responses[k].openPositions[i];

            let added = false;
            for (let j = 0; j < this.openAssetsId.length; ++j) {
              if (this.openAssetsId[j] == openPosition.assetId) {
                added = true;
                break;
              }
            }
            if (!added) {
              openAssetsPair.push(openPosition.pair);
              this.openAssetsId.push(openPosition.assetId);
              this.assetData[openPosition.assetId] = { name: openPosition.assetName, code: openPosition.assetCode };
            }
          }
        }
      }
    }
    return openAssetsPair;
  }

  private setClosePositions(response: AdvisorResponse) : void {
    this.closePosition[response.userId] = new PositionResponse();
    this.closePosition[response.userId].orderCount = 0;
    this.closePosition[response.userId].successCount = 0;
    this.closePosition[response.userId].totalInvested = 0;
    this.closePosition[response.userId].totalProfit = 0;
    this.closePosition[response.userId].totalVirtual = 0;
    this.closePosition[response.userId].totalQuantity = 0;
    this.closePosition[response.userId].successRate = 0;
    this.closePosition[response.userId].averagePrice = 0;
    this.closePosition[response.userId].totalFee = 0;
    this.closeAssetPosition[response.userId] = [];
    let totalTradeMinutes = 0;
    let totalAssetTradeMinutes: { [assetId: number]: number } = {};
    let closedAssetData: { [assetId: number]: AssetPositionResponse} = {};
    let closedAssetsId = [];
    if (response.closedPositions) {
      for (let i = 0; i < response.closedPositions.length; ++i) {
        if (!closedAssetData[response.closedPositions[i].assetId]) {
          closedAssetsId.push(response.closedPositions[i].assetId);
          closedAssetData[response.closedPositions[i].assetId] = new AssetPositionResponse();
          closedAssetData[response.closedPositions[i].assetId].assetId = response.closedPositions[i].assetId;
          closedAssetData[response.closedPositions[i].assetId].assetName = response.closedPositions[i].assetName;
          closedAssetData[response.closedPositions[i].assetId].assetCode = response.closedPositions[i].assetCode;
          closedAssetData[response.closedPositions[i].assetId].positionResponse = new PositionResponse();
          closedAssetData[response.closedPositions[i].assetId].positionResponse.orderCount = 0;
          closedAssetData[response.closedPositions[i].assetId].positionResponse.successCount = 0;
          closedAssetData[response.closedPositions[i].assetId].positionResponse.totalInvested = 0;
          closedAssetData[response.closedPositions[i].assetId].positionResponse.totalProfit = 0;
          closedAssetData[response.closedPositions[i].assetId].positionResponse.totalVirtual = 0;
          closedAssetData[response.closedPositions[i].assetId].positionResponse.totalQuantity = 0;
          closedAssetData[response.closedPositions[i].assetId].positionResponse.successRate = 0;
          closedAssetData[response.closedPositions[i].assetId].positionResponse.averagePrice = 0;
          closedAssetData[response.closedPositions[i].assetId].positionResponse.totalFee = 0;
          if (!totalAssetTradeMinutes[response.closedPositions[i].assetId]) {
            totalAssetTradeMinutes[response.closedPositions[i].assetId] = 0;
          }
        }
        closedAssetData[response.closedPositions[i].assetId].positionResponse.orderCount += response.closedPositions[i].orderCount;
        closedAssetData[response.closedPositions[i].assetId].positionResponse.successCount += response.closedPositions[i].successCount;
        closedAssetData[response.closedPositions[i].assetId].positionResponse.totalInvested += response.closedPositions[i].totalInvested;
        closedAssetData[response.closedPositions[i].assetId].positionResponse.totalProfit += response.closedPositions[i].totalProfit;
        closedAssetData[response.closedPositions[i].assetId].positionResponse.totalQuantity += response.closedPositions[i].totalQuantity;
        closedAssetData[response.closedPositions[i].assetId].positionResponse.totalVirtual += response.closedPositions[i].totalVirtual;
        closedAssetData[response.closedPositions[i].assetId].positionResponse.totalFee += response.closedPositions[i].totalFee;

        this.closePosition[response.userId].orderCount += response.closedPositions[i].orderCount;
        this.closePosition[response.userId].successCount += response.closedPositions[i].successCount;
        this.closePosition[response.userId].totalInvested += response.closedPositions[i].totalInvested;
        this.closePosition[response.userId].totalProfit += response.closedPositions[i].totalProfit;
        this.closePosition[response.userId].totalQuantity += response.closedPositions[i].totalQuantity;
        this.closePosition[response.userId].totalVirtual += response.closedPositions[i].totalVirtual;
        this.closePosition[response.userId].totalFee += response.closedPositions[i].totalFee;
        if (response.closedPositions[i].summedTradeMinutes) {
          totalTradeMinutes += response.closedPositions[i].summedTradeMinutes;
          totalAssetTradeMinutes[response.closedPositions[i].assetId] += response.closedPositions[i].summedTradeMinutes;
        }
      }
      if (response.closedPositions.length > 0) {
        this.closePosition[response.userId].successRate = this.closePosition[response.userId].successCount / this.closePosition[response.userId].orderCount;
        this.closePosition[response.userId].averageReturn = this.closePosition[response.userId].totalProfit / this.closePosition[response.userId].totalInvested;
        this.closePosition[response.userId].averagePrice = this.closePosition[response.userId].totalInvested / this.closePosition[response.userId].totalQuantity;
        if (totalTradeMinutes > 0) {
          this.closePosition[response.userId].averageTradeMinutes = totalTradeMinutes / this.closePosition[response.userId].orderCount;
        } 
      }
      for (let i = 0; i < closedAssetsId.length; ++i) {
        closedAssetData[closedAssetsId[i]].positionResponse.successRate = closedAssetData[closedAssetsId[i]].positionResponse.successCount / closedAssetData[closedAssetsId[i]].positionResponse.orderCount;
        closedAssetData[closedAssetsId[i]].positionResponse.averageReturn = closedAssetData[closedAssetsId[i]].positionResponse.totalProfit / closedAssetData[closedAssetsId[i]].positionResponse.totalInvested;
        closedAssetData[closedAssetsId[i]].positionResponse.averagePrice = closedAssetData[closedAssetsId[i]].positionResponse.totalInvested / closedAssetData[closedAssetsId[i]].positionResponse.totalQuantity;
        if (totalAssetTradeMinutes[closedAssetsId[i]] > 0) {
          closedAssetData[closedAssetsId[i]].positionResponse.averageTradeMinutes = totalAssetTradeMinutes[closedAssetsId[i]] / closedAssetData[closedAssetsId[i]].positionResponse.orderCount;
        }
        this.closeAssetPosition[response.userId].push(closedAssetData[closedAssetsId[i]]);
      }
    }
    if (!this.advisorListData) {
      this.closePositionResponseSubject.next(this.closePosition[response.userId]);
      this.allAssetsClosePositionAssetResponseSubject.next(this.closeAssetPosition[response.userId]);
    }
  }

  private setBinanceTicker(pair: PairResponse) : void {
    if (pair.symbol) {
      this.binanceTickers.push(this.tickerService.binanceTicker(pair.symbol).subscribe(ret => 
      {
        this.pairPrice[ret.pair] = {};
        this.pairPrice[ret.pair][Constants.OrderType.Buy] = ret.bestAskPrice;
        this.pairPrice[ret.pair][Constants.OrderType.Sell] = ret.bestBidPrice;
        this.internalRefresh();
      }));
    }
    if (pair.multipliedSymbol) {
      this.binanceTickers.push(this.tickerService.binanceTicker(pair.multipliedSymbol).subscribe(ret => 
      {
        this.pairPrice[ret.pair] = {};
        this.pairPrice[ret.pair][Constants.OrderType.Buy] = ret.bestAskPrice;
        this.pairPrice[ret.pair][Constants.OrderType.Sell] = ret.bestBidPrice;
        this.internalRefresh();
      }));
    }
  }

  private internalRefresh() : void {
    this.openPositionAssetResponse = {};
    if (!this.advisorListData) {
      this.refreshAllData(this.advisorData);
    } else {
      for (let i = 0; i < this.advisorListData.length; ++i) {
        this.refreshAllData(this.advisorListData[i]);
      }
      this.advisorListResponseSubject.next(this.advisorListData);
    }
  }

  private refreshAllData(response: AdvisorResponse) : void {
    let openPositionResponse = new PositionResponse();
    openPositionResponse.orderCount = 0;
    openPositionResponse.successCount = 0;
    openPositionResponse.totalInvested = 0;
    openPositionResponse.totalProfit = 0;
    openPositionResponse.totalVirtual = 0;
    openPositionResponse.totalQuantity = 0;
    openPositionResponse.successRate = 0;
    openPositionResponse.averagePrice = 0;
    openPositionResponse.totalFee = 0;
    this.setOpenPositionAssetResponse(response);
    this.setOpenPositions(response.userId, openPositionResponse);

    let totalProfit = openPositionResponse.totalProfit + this.closePosition[response.userId].totalProfit;
    let totalInvested = openPositionResponse.totalInvested + this.closePosition[response.userId].totalInvested;
    if (totalInvested > 0) {
      response.averageReturn = totalProfit / totalInvested;
    }
    response.equity = openPositionResponse.totalProfit + response.totalBalance + response.totalAllocated;
    response.totalProfit = response.equity - CONFIG.virtualMoney;
    response.totalProfitPercentage = response.equity / CONFIG.virtualMoney - 1;
    if (response.lastPortfolioValue) {
      response.profit24hValue = response.equity - response.lastPortfolioValue;
      response.profit24hPercentage = response.equity / response.lastPortfolioValue - 1;
    }
    if (response.monthlyRankingHistory && response.monthlyRankingHistory.portfolioValue) {
      response.monthlyRankingHistory.profitPercentage = response.equity / response.monthlyRankingHistory.portfolioValue - 1;
    }
    if (!this.advisorListData) {
      this.advisorResponseSubject.next(response);
    }
  }

  private setOpenPositionAssetResponse(response: AdvisorResponse) : void {
    this.openPositionAssetResponse[response.userId] = {};
    if (response.openPositions) {
      for (let i = 0; i < response.openPositions.length; ++i) {
        let openPosition = response.openPositions[i];
        let averageReturn = openPosition.averageReturn;
        if (this.pairPrice[openPosition.pair.symbol.toUpperCase()]) {
          let basePrice = this.pairPrice[openPosition.pair.symbol.toUpperCase()][!!openPosition.type ? Constants.OrderType.Sell : Constants.OrderType.Buy];
          if (basePrice) {
            if (!openPosition.pair.multipliedSymbol) {
              averageReturn = Util.GetProfit(openPosition.type, openPosition.averagePrice, openPosition.totalFee, openPosition.totalQuantity, openPosition.totalQuantity, basePrice);
            } else if (this.pairPrice[openPosition.pair.multipliedSymbol.toUpperCase()]) {
              let relatedPrice = this.pairPrice[openPosition.pair.multipliedSymbol.toUpperCase()][!!openPosition.type ? Constants.OrderType.Sell : Constants.OrderType.Buy];
              if (relatedPrice) {
                averageReturn = Util.GetProfit(openPosition.type, openPosition.averagePrice, openPosition.totalFee, openPosition.totalQuantity, openPosition.totalQuantity, (basePrice * relatedPrice));
              }
            }
          }
        }
        if (!this.openPositionAssetResponse[response.userId][openPosition.assetId]) {
          this.openPositionAssetResponse[response.userId][openPosition.assetId] = new Array(2);
        }
        this.openPositionAssetResponse[response.userId][openPosition.assetId][openPosition.type] = new PositionResponse();
        this.openPositionAssetResponse[response.userId][openPosition.assetId][openPosition.type].successRate = openPosition.successRate;
        this.openPositionAssetResponse[response.userId][openPosition.assetId][openPosition.type].orderCount = openPosition.orderCount;
        this.openPositionAssetResponse[response.userId][openPosition.assetId][openPosition.type].successCount = openPosition.successCount;
        this.openPositionAssetResponse[response.userId][openPosition.assetId][openPosition.type].totalInvested = openPosition.totalInvested;
        this.openPositionAssetResponse[response.userId][openPosition.assetId][openPosition.type].totalQuantity = openPosition.totalQuantity;
        this.openPositionAssetResponse[response.userId][openPosition.assetId][openPosition.type].averagePrice = openPosition.averagePrice;
        this.openPositionAssetResponse[response.userId][openPosition.assetId][openPosition.type].totalFee = openPosition.totalFee;
        this.openPositionAssetResponse[response.userId][openPosition.assetId][openPosition.type].averageReturn = averageReturn; 
        this.openPositionAssetResponse[response.userId][openPosition.assetId][openPosition.type].totalProfit = averageReturn * openPosition.totalInvested; 
        this.openPositionAssetResponse[response.userId][openPosition.assetId][openPosition.type].totalVirtual = averageReturn * openPosition.totalInvested + openPosition.totalInvested; 
      }
    }
  } 

  private setOpenPositions(advisorId: number, openPositionResponse: PositionResponse) : void {
    let totalOverallQuantity = 0;
    let totalTypeRelatedQuantity = 0;
    let allAssetsOpenPositionAssetResponses = [];
    for (let i = 0; i < this.openAssetsId.length; ++i) {
      let orderCount = 0;
      let successCount = 0;
      let totalInvested = 0;
      let totalProfit = 0;
      let totalVirtual = 0;
      let totalQuantity = 0;
      let totalFee = 0;
      let typeRelatedQuantity = 0;
      if (this.openPositionAssetResponse[advisorId][this.openAssetsId[i]]) {
        for (let j = 0; j < this.openPositionAssetResponse[advisorId][this.openAssetsId[i]].length; ++j) {
          if (this.openPositionAssetResponse[advisorId][this.openAssetsId[i]][j]) {
            orderCount += this.openPositionAssetResponse[advisorId][this.openAssetsId[i]][j].orderCount;
            successCount += this.openPositionAssetResponse[advisorId][this.openAssetsId[i]][j].successCount;
            totalInvested += this.openPositionAssetResponse[advisorId][this.openAssetsId[i]][j].totalInvested;
            totalProfit += this.openPositionAssetResponse[advisorId][this.openAssetsId[i]][j].totalProfit;
            totalVirtual += this.openPositionAssetResponse[advisorId][this.openAssetsId[i]][j].totalVirtual;
            totalQuantity += this.openPositionAssetResponse[advisorId][this.openAssetsId[i]][j].totalQuantity;
            totalFee += this.openPositionAssetResponse[advisorId][this.openAssetsId[i]][j].totalFee;
            typeRelatedQuantity += (j == Constants.OrderType.Sell ? -1.0 : 1.0) * this.openPositionAssetResponse[advisorId][this.openAssetsId[i]][j].totalQuantity;
          }
        }
      }
      totalOverallQuantity += totalQuantity;
      totalTypeRelatedQuantity += typeRelatedQuantity;
      
      openPositionResponse.orderCount += orderCount;
      openPositionResponse.successCount += successCount;
      openPositionResponse.totalInvested += totalInvested;
      openPositionResponse.totalProfit += totalProfit;
      openPositionResponse.totalVirtual += totalVirtual;
      openPositionResponse.totalFee += totalFee;
      openPositionResponse.totalQuantity += Math.abs(typeRelatedQuantity);

      if (!this.advisorListData) {
        let position = new PositionResponse();
        position.type = typeRelatedQuantity >= 0 ? Constants.OrderType.Buy : Constants.OrderType.Sell;
        position.orderCount = orderCount;
        position.successCount = successCount;
        position.successRate = successCount / orderCount;
        position.totalInvested = totalInvested;
        position.totalProfit = totalProfit;
        position.totalVirtual = totalVirtual;
        position.totalFee = totalFee;
        position.totalQuantity = Math.abs(typeRelatedQuantity);
        position.averageReturn = totalProfit / totalInvested;
        position.averagePrice = totalInvested / totalQuantity;
        let assetPosition = new AssetPositionResponse();
        assetPosition.assetId = this.openAssetsId[i];
        assetPosition.positionResponse = position;
        if (this.assetData[this.openAssetsId[i]]) {
          assetPosition.assetCode = this.assetData[this.openAssetsId[i]].code;
          assetPosition.assetName = this.assetData[this.openAssetsId[i]].name;
        }
        allAssetsOpenPositionAssetResponses.push(assetPosition);
        if (!this.openPositionAssetResponseSubject[this.openAssetsId[i]]) {
          this.openPositionAssetResponseSubject[this.openAssetsId[i]] = new Subject<AssetPositionResponse>();
        } 
        this.openPositionAssetResponseSubject[this.openAssetsId[i]].next(assetPosition);
      }
    }
    this.allAssetsOpenPositionAssetResponseSubject.next(allAssetsOpenPositionAssetResponses);
    if (openPositionResponse.orderCount > 0) {
      openPositionResponse.successRate = openPositionResponse.successCount / openPositionResponse.orderCount;
      openPositionResponse.averageReturn = openPositionResponse.totalProfit / openPositionResponse.totalInvested;
      openPositionResponse.averagePrice = openPositionResponse.totalInvested / totalOverallQuantity;
      openPositionResponse.type = totalTypeRelatedQuantity >= 0 ? Constants.OrderType.Buy : Constants.OrderType.Sell;
    }
    if (!this.advisorListData) {
      this.openPositionResponseSubject.next(openPositionResponse);
    }
  }
}