export class RequestToBeAdvisorRequest {
  name: string;
  description: string;
  previousExperience: string;
  email: string;
  password: string;
  changedPicture: boolean;
  captcha: string;
  file: File;
  
  constructor(){
  }
}
