import { PairResponse } from "../asset/assetResponse";

export class OrderResponse {
    id: number;
    creationDate: Date;
    advisorId: number;
    advisorName: string;
    advisorDescription: string;
    advisorGuid: string;
    advisorRanking?: number;
    advisorRating?: number;
    followingAdvisor: boolean;
    assetId: number;
    assetCode: string;
    assetName: string;
    shortSellingEnabled?: boolean;
    pair: PairResponse;
    followingAsset: boolean;
    actionType: number;
    type: number;
    status: number;
    statusDate: Date;
    quantity: number;
    remainingQuantity: number;
    price: number;
    invested: number;
    takeProfit?: number;
    stopLoss?: number;
    profit?: number;
    profitValue?: number;
    profitWithoutFee?: number;
    profitWithoutFeeValue?: number;
    fee?: number;
    orderId?: number;
    openDate?: Date;
    openPrice?: number;
    canBeEdited: boolean;
}