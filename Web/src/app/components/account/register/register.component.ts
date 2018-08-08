import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
 /* registerRequest: LoginRequest = new LoginRequest();
  loginPromise: Subscription;
  loginForm: FormGroup;*/

  constructor(/*private formBuilder: FormBuilder, 
    private accountService: AccountService, 
    private router: Router,
  private notificationsService: NotificationsService*/) { }

  ngOnInit() {
  } 

}
