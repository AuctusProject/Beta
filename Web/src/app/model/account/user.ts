import { RequestToBeAdvisor } from "../advisor/requestToBeAdvisor";

export class User {
  id: number;
  email: string;
  password: string;
  confirmationDate? : Date;
  confirmationCode: string;
  isAdvisor: boolean;
  requestToBeAdvisor: RequestToBeAdvisor;

  constructor(){
  }
}
