export class ReportResponse {
  reportId: number;
  reportDate: Date;
  assetId: number;
  agencyId: number;
  agencyName: number;
  agencyWebSite: number;
  rate: RatingDetail;
  rateOptions: RatingDetail[];
}

export class RatingDetail
{
    rate: string;
    description: string;
    hexaColor: string;
}