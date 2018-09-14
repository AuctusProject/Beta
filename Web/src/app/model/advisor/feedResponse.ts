export class FeedResponse {
  adviceId: number;
  assetId: number;
  assetName: string;
  assetCode: string;
  advisorId: number;
  advisorName: string;
  advisorUrlGuid: string;
  advisorRanking: number;
  advisorRating: number;
  followingAdvisor: boolean;
  followingAsset: boolean;
  adviceType: number;
  adviceDate: Date;
  assetValueAtAdviceTime: number;
}
