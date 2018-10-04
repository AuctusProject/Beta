export class ReportResponse {
  reportId: number;
  reportDate: Date;
  assetId: number;
  agencyId: number;
  agencyName: number;
  agencyWebSite: number;
  //rate: string;
  rate: RatingDetail;
  //rateDetail: RatingDetail;
  rateOptions: RatingDetail[];
  score: string;
}

export class RatingDetail
{
    rate: string;
    description: string;
    hexaColor: string;
}