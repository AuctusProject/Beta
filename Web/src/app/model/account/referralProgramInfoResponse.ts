export class ReferralProgramInfoResponse {
  referralCode: string;
  pending: number;
  canceled: number;
  cashedOut: number;
  available: number;
  bonusToReferred: number;

  constructor(){
  }
}
