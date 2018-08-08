import { Component, OnInit } from '@angular/core';

import { RegisterRequest } from '../../../model/account/registerRequest';
//import { ResgisterResponse } from '../../../model/account/registerResponse';
@Component({
  selector: 'register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  // registerRequest: RegisterRequest = new RegisterRequest();
  // registerPromise: Subscription;
  // registerForm: FormGroup;

  constructor(/*private formBuilder: FormBuilder, 
    private accountService: AccountService, 
    private router: Router,
  private notificationsService: NotificationsService*/) { }

  ngOnInit() {
  } 

}
