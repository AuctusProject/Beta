export class OrderRequest {
    assetId: number;
    type: number;
    quantity: number;
    price?: number;
    takeProfit?: number;
    stopLoss?: number;
}