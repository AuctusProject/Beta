import { ReportResponse } from "../asset/reportResponse";

export class FeedResponse {
  assetId: number;
  assetName: string;
  assetCode: string;
  assetMode: number;
  followingAsset: boolean;
  date: Date;
  advice: AdviceResponse;
  report: ReportResponse;
}

export class AdviceResponse{
  advisorId: number;
  advisorName: string;
  advisorUrlGuid: string;
  advisorRanking: number;
  advisorRating: number;
  followingAdvisor: boolean;
  adviceId: number;
  adviceType: number;
  assetValueAtAdviceTime: number;
}
