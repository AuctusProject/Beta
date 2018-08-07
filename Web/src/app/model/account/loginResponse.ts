import { LoginData } from "./loginData";

export class LoginResponse {
  email: string;
  logged: boolean;
  error: string;
  data: LoginData;

  constructor(){
  }
}
