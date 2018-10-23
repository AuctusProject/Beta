export class RegisterAdvisorRequest {
  name: string;
  description: string;
  email: string;
  password: string;
  changedPicture: boolean;
  captcha: string;
  file: File;
  
  constructor(){
  }
}
