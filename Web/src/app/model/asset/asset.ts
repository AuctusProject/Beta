import { PairResponse } from "./assetResponse";

export class Asset {
  id: number;
  name: string;
  code: string;
  type: number;
  shortSellingEnabled: boolean;
  marketCap: number;
  pair: PairResponse;
}