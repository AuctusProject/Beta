import { Component, OnInit } from '@angular/core';
import { RegisterRequest } from '../../../model/account/registerRequest';
import { RegisterResponse } from '../../../model/account/registerResponse';
import { Subscription } from '../../../../../node_modules/rxjs';
import { FormGroup, FormBuilder, Validators, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { AccountService } from '../../../services/account.service';
import { Router, ActivatedRoute } from '../../../../../node_modules/@angular/router';
import { NotificationsService } from '../../../../../node_modules/angular2-notifications';
import { AuthRedirect } from '../../../providers/authRedirect';

@Component({
  selector: 'register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
 
  registerRequest: RegisterRequest = new RegisterRequest();
  registerPromise: Subscription;
  registerForm: FormGroup;

  constructor(private formBuilder: FormBuilder,
    private notificationsService: NotificationsService,
    private accountService: AccountService,
    private authRedirect : AuthRedirect,
    private router: Router,
    private activatedRoute: ActivatedRoute) {
    this.buildForm();
  }

  ngOnInit() {
    this.registerRequest.referralCode = this.activatedRoute.snapshot.queryParams['ref'];
  }

  private buildForm() {
    this.registerForm = this.formBuilder.group({
      email: ['', Validators.compose([Validators.required])],
      password: ['', Validators.compose([Validators.required])],
      referralCode: [''],
      requestedToBeAdvisor: [''],
    });
  }

  onCreateAccountClick(){
    this.createAccount();
  }

  createAccount(){
    this.registerPromise = this.accountService.register(this.registerRequest)
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
