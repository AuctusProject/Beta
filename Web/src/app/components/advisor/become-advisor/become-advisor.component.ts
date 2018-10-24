import { Component, OnInit, EventEmitter, Input, Output, ViewChild } from '@angular/core';
import { RequestToBeAdvisor } from '../../../model/advisor/requestToBeAdvisor';
import { AdvisorService } from '../../../services/advisor.service';
import { RegisterAdvisorRequest } from '../../../model/advisor/registerAdvisorRequest';
import { NotificationsService } from 'angular2-notifications';
import { ModalComponent } from '../../../model/modal/modalComponent';
import { FullscreenModalComponentInput } from '../../../model/modal/fullscreenModalComponentInput';
import { Subscription } from 'rxjs';
import { RecaptchaComponent } from '../../util/recaptcha/recaptcha.component';
import { FileUploaderComponent } from '../../util/file-uploader/file-uploader.component';
import { CONFIG } from '../../../services/config.service';
import { AccountService } from '../../../services/account.service';
import { MessageFullscreenModalComponent } from '../../util/message-fullscreen-modal/message-fullscreen-modal.component';
import { InheritanceInputComponent } from '../../util/inheritance-input/inheritance-input.component';
import { InheritanceInputOptions, InputType } from '../../../model/inheritanceInputOptions';
import { AuthRedirect } from '../../../providers/authRedirect';
import { LocalStorageService } from 'src/app/services/local-storage.service';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'become-advisor',
  templateUrl: './become-advisor.component.html',
  styleUrls: ['./become-advisor.component.css']
})
export class BecomeAdvisorComponent implements ModalComponent, OnInit {
  modalTitle: string = "Registration";
  @Input() data: any;
  @Output() setClose = new EventEmitter<void>();
  @Output() setNewModal = new EventEmitter<FullscreenModalComponentInput>();
  
  registerAdvisorRequest: RegisterAdvisorRequest = new RegisterAdvisorRequest();
  promise: Subscription;
  @ViewChild("RecaptchaComponent") RecaptchaComponent: RecaptchaComponent;
  @ViewChild("FileUploadComponent") FileUploadComponent: FileUploaderComponent;
  @ViewChild("Name") Name: InheritanceInputComponent;
  @ViewChild("Email") Email: InheritanceInputComponent;
  @ViewChild("Password") Password: InheritanceInputComponent;
  @ViewChild("Description") Description: InheritanceInputComponent;
  @ViewChild("Referral") Referral: InheritanceInputComponent;
  
  completeRegistration: boolean = false;
  showReferralInput: boolean = true;
  validReferral: boolean = false;

  constructor(private advisorService: AdvisorService, 
    private accountService: AccountService,
    private notificationsService: NotificationsService,
    private authRedirect: AuthRedirect,
    private activatedRoute: ActivatedRoute,
    private localStorageService: LocalStorageService) { }

  ngOnInit() {
    if (this.accountService.getLoginData().isAdvisor) {
      this.setClose.emit();
      this.authRedirect.redirectAfterLoginAction();
    } else {
      this.completeRegistration = this.data && this.data.completeregistration;
      this.registerAdvisorRequest.email = this.isNewUser() ? "" : this.accountService.getLoginData().email;
      this.showReferralInput = this.isNewUser() || !this.completeRegistration;
      this.registerAdvisorRequest.password = "";
      this.registerAdvisorRequest.referralCode = this.activatedRoute.snapshot.queryParams['ref'];
      if (!this.registerAdvisorRequest.referralCode) {
        this.registerAdvisorRequest.referralCode = this.localStorageService.getLocalStorage("referralCode");
        if (this.registerAdvisorRequest.referralCode) {
          this.showReferralInput = true;
        }
      } 
      if (!!this.registerAdvisorRequest.referralCode) {
        this.validateReferralCode(this.registerAdvisorRequest.referralCode);
      }
    }
  }

  isNewUser() {
    return !this.accountService.isLoggedIn();
  }

  public onCaptchaResponse(captchaResponse: string) {
    this.registerAdvisorRequest.captcha = captchaResponse;
  }

  getSubtitleText() : string {
    return this.completeRegistration ? "Please fill your data and become an Expert." : "Sign up to get exclusive market insights, as well as access to expert knowledge.";
  }

  onSubmit() {
    if (this.isNewUser() && !this.registerAdvisorRequest.captcha) {
      this.notificationsService.error(null, "You must fill the captcha.");
    } else if (this.isValidRequest()) {
      this.registerAdvisorRequest.changedPicture = this.FileUploadComponent.fileWasChanged();
      this.registerAdvisorRequest.file = this.FileUploadComponent.getFile();
      this.promise = this.advisorService.postRegisterAdvisor(this.registerAdvisorRequest).subscribe(result => 
      {
        if (!!result && !result.error && result.data) {
          this.accountService.setLoginData(result.data);
          this.setClose.emit();
          this.authRedirect.redirectAfterLoginAction(result.data);
        } else if (!!result && result.error) {
          this.notificationsService.info("Info", result.error);
        }
      }, error =>
      {
        if (!!this.RecaptchaComponent) this.RecaptchaComponent.reset();
      });
    }
  }

  isValidRequest() : boolean {
    let isValid = this.Name.isValid();
    isValid = this.Description.isValid() && isValid;
    if (this.isNewUser()) {
      isValid = this.Email.isValid() && isValid;
      isValid = this.Referral.isValid() && isValid;
      isValid = this.Password.isValid() && isValid;
    } else if (!this.completeRegistration) {
      isValid = this.Referral.isValid() && isValid;
    }
    return isValid;
  }

  onChangeReferralCode(value: string) {
    this.registerAdvisorRequest.referralCode = value;
    this.validateReferralCode(value);
  }

  validateReferralCode(value: string) {
    if (!value || value.length == 0) {
      this.setInvalidReferral("");
    } else if (!!value && value.length == 7) {
      this.accountService.isValidReferralCode(value).subscribe(response => {
        if (!!response && response.valid) {
          this.validReferral = true;
          this.Referral.setForcedError("");
          this.localStorageService.setLocalStorage("referralCode", value);
        } else {
          this.setInvalidReferral("Invalid referral code");
        }
      });
    } else {
      this.setInvalidReferral("Invalid referral code");
    }
  }

  setInvalidReferral(message: string) {
    this.validReferral = false;
    this.Referral.setForcedError(message);
    this.localStorageService.setLocalStorage("referralCode", "");
  }

  getNameOptions() {
    return { textOptions: { placeHolder: "Name", browserAutocomplete: "name", maxLength: 50 } };
  }

  getEmailOptions() {
    return { inputType: InputType.Email, textOptions: { disabled: !this.isNewUser(), placeHolder: "Email", showHintSize: false, maxLength: 50 } };
  }

  getPasswordOptions() {
    return { inputType: InputType.Password, textOptions: { placeHolder: "Password", browserAutocomplete: "off", minLength: 8, maxLength: 100 } };
  }

  getDescriptionOptions() {
    return { textOptions: { placeHolder: "Short description", maxLength: 160, required: false } };
  }

  getReferralOptions() {
    return { textOptions: { outlineField: false, placeHolder: "Referral code (optional)", required: false, showHintSize: false, minLength: 7, maxLength: 7 } };
  }
}
