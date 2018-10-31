export class AdviseRequest {
  assetId: number;
  adviceType: number;
  targetPrice?: number;
  stopLoss?: number;
  currentValue?: number;
}
