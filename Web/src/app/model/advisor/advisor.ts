import { AdvisorDetails } from "./advisorDetails";

export class Advisor {
  id: number;
  name: string;
  description: string;
  urlPhoto: string;
  advisorDetails: AdvisorDetails;

  constructor(){
    this.advisorDetails = new AdvisorDetails();
  }
}
