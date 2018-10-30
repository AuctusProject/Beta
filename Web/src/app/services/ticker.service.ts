import { BinanceTickerPayload } from "../model/binanceTickerPayload";
import { Injectable } from "@angular/core";
import { empty, Subject, Observable } from "rxjs";
import { BinanceWebSocket } from "../util/binanceWebSocket";

@Injectable({ providedIn: 'root' })
export class TickerService {
  private binanceWebSocketInitialized: boolean;
  private binanceTickerSource: { [pair: string]: Subject<BinanceTickerPayload>; } = {};
  
  public initialize() : void {
    if (!this.binanceWebSocketInitialized) {
      this.binanceWebSocketInitialized = true;
      BinanceWebSocket.Initialize((data) => { this.getBinanceWebSocketPayload(data) });
    }
  }

  public binanceTicker(pair: string) : Observable<BinanceTickerPayload> {
    if (!pair) {
      return empty();
    } else if (!this.binanceTickerSource[pair.toLowerCase()]) {
      this.binanceTickerSource[pair.toLowerCase()] = new Subject<BinanceTickerPayload>();
    } 
    return this.binanceTickerSource[pair.toLowerCase()].asObservable();
  }

  private setBinanceTicker(ticker: BinanceTickerPayload) : void {
    if (!this.binanceTickerSource[ticker.pair.toLowerCase()]) {
      this.binanceTickerSource[ticker.pair.toLowerCase()] = new Subject<BinanceTickerPayload>();
    }
    this.binanceTickerSource[ticker.pair.toLowerCase()].next(ticker);
  }

  private getBinanceWebSocketPayload(data: string): void {
    if (data) {
      let payload = new BinanceTickerPayload();
      JSON.parse(data, (p, v) =>
      {
        if (p == "e") {
          payload.eventType = v;
        } else if (p == "E") {
          payload.eventTime = new Date(v);
        } else if (p == "s") {
          payload.pair = v;
        } else if (p == "p") {
          payload.priceChange = parseFloat(v);
        } else if (p == "P") {
          payload.priceChangePercentage = parseFloat(v);
        } else if (p == "w") {
          payload.weightedAveragePrice = parseFloat(v);   
        } else if (p == "x") {
          payload.previousDayClosePrice = parseFloat(v);   
        } else if (p == "c") {
          payload.currentClosePrice = parseFloat(v);   
        } else if (p == "Q") {
          payload.closeTradeQty = parseFloat(v);   
        } else if (p == "b") {
          payload.bestBidPrice = parseFloat(v);    
        } else if (p == "B") {
          payload.bestBidQty = parseFloat(v); 
        } else if (p == "a") {
          payload.bestAskPrice = parseFloat(v);   
        } else if (p == "A") {
          payload.bestAskQty = parseFloat(v);    
        } else if (p == "o") {
          payload.openPrice = parseFloat(v);   
        } else if (p == "h") {
          payload.highPrice = parseFloat(v);    
        } else if (p == "l") {
          payload.lowPrice = parseFloat(v); 
        } else if (p == "v") {
          payload.totalVolumeBaseAsset = parseFloat(v);
        } else if (p == "q") {
          payload.totalVolumeQuoteAsset = parseFloat(v);  
        } else if (p == "O") {
          payload.statOpenTime = v;  
        } else if (p == "C") {
          payload.statCloseTime = v;    
        } else if (p == "F") {
          payload.firstTradeId = v;  
        } else if (p == "L") {
          payload.lastTradeId = v; 
        } else if (p == "n") {
          payload.totalTrades = v; 
        } else if (p != "" && parseInt(p, 10) >= 0) {
          this.setBinanceTicker(payload);
          payload = new BinanceTickerPayload();
        }
      });
    }
  }
}