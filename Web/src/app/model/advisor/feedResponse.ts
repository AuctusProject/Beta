import { ReportResponse } from "../asset/reportResponse";
import { EventResponse } from "../asset/eventResponse";

export class FeedResponse {
  assetId: number;
  assetName: string;
  assetCode: string;
  assetMode: number;
  followingAsset: boolean;
  date: Date;
  advice: AdviceResponse;
  report: ReportResponse;
  event: EventResponse;
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
