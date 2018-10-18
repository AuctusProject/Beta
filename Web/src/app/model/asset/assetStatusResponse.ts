export class AssetStatusResponse {
    assetId: number;
    code: string;
    name: string;
    price?: number;
    marketCap?: number;
    marketCapRank?: number;
    totalVolume?: number;
    high24h?: number;
    low24h?: number;
    variation24h?: number;
    variationPercentage24h?: number;
    marketCapVariation24h?: number;
    marketCapPercentage24h?: number;
    circulatingSupply?: number;
    totalSupply?: number;
    allTimeHigh?: number;
    allTimeHighPercentage?: number;
    allTimeHighDate?: Date;
}