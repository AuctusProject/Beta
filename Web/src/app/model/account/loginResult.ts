import { LoginResponse } from "./loginResponse";

export class LoginResult {
  email: string;
  logged: boolean;
  error: string;
  data: LoginResponse;

  constructor(){
  }
}
