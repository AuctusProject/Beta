export class BinanceTickerPayload {
    eventType: string; // Event type
    eventTime: Date; // Event time
    pair: string; // Symbol
    priceChange: number; // Price change
    priceChangePercentage: number; // Price change percent
    weightedAveragePrice: number; // Weighted average price
    previousDayClosePrice: number; // Previous day's close price
    currentClosePrice: number; // Current day's close price
    closeTradeQty: number; // Close trade's quantity
    bestBidPrice: number; // Best bid price
    bestBidQty: number; // Best bid quantity
    bestAskPrice: number; // Best ask price
    bestAskQty: number; // Best ask quantity
    openPrice: number; // Open price
    highPrice: number; // High price
    lowPrice: number; // Low price
    totalVolumeBaseAsset: number; // Total traded base asset volume
    totalVolumeQuoteAsset: number; // Total traded quote asset volume
    statOpenTime: number; // Statistics open time
    statCloseTime: number; // Statistics close time
    firstTradeId: number; // First trade ID
    lastTradeId: number; // Last trade Id
    totalTrades: number; // Total number of trades
  }