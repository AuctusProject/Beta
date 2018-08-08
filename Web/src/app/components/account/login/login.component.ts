import { Component, OnInit, Input } from '@angular/core';
import { AccountService } from '../../../services/account.service';
import { LoginRequest } from '../../../model/account/loginRequest';
import { Router } from '../../../../../node_modules/@angular/router';
import { Subscription } from '../../../../../node_modules/rxjs';
import { FormGroup, FormBuilder, Validators, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NotificationsService } from '../../../../../node_modules/angular2-notifications';
import { AuthRedirect } from '../../../providers/authRedirect';

@Component({
  selector: 'login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {
  loginRequest: LoginRequest = new LoginRequest();
  loginPromise: Subscription;
  loginForm: FormGroup;

  constructor(private formBuilder: FormBuilder, 
    private accountService: AccountService, 
    private router: Router,
    private notificationsService: NotificationsService,
    private authRedirect : AuthRedirect) { 
    this.buildForm();
  }

  private buildForm() {
    this.loginForm = this.formBuilder.group({
      email: ['', Validators.compose([Validators.required])],
      password: ['', Validators.compose([Validators.required])]
    });
  }

  ngOnInit() {
  }

  onLoginClick(){
    this.doLogin();
  }

  doLogin() {
    this.loginPromise = this.accountService.login(this.loginRequest)
      .subscribe(response => {
        if (response){
          if (!response.error && response.data) {
            this.accountService.setLoginData(response.data);
            this.authRedirect.redirectAfterLoginAction();
          }
          else {
            this.notificationsService.info("Info", response.error);
          }
        }
      });
  }
}
