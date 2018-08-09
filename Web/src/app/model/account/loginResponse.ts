export class LoginResponse {
  id: number;
  email: string;
  hasInvestment : boolean;
  isAdvisor : boolean;
  pendingConfirmation : boolean;
  requestedToBeAdvisor : boolean; 

  constructor(){
  }
}
