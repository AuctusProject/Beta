import { Component, OnInit, EventEmitter, Input, Output, ViewChild } from '@angular/core';
import { RequestToBeAdvisor } from '../../../model/advisor/requestToBeAdvisor';
import { AdvisorService } from '../../../services/advisor.service';
import { RequestToBeAdvisorRequest } from '../../../model/advisor/requestToBeAdvisorRequest';
import { NotificationsService } from '../../../../../node_modules/angular2-notifications';
import { ModalComponent } from '../../../model/modal/modalComponent';
import { FullscreenModalComponentInput } from '../../../model/modal/fullscreenModalComponentInput';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { Subscription } from 'rxjs';
import { RecaptchaComponent } from '../../util/recaptcha/recaptcha.component';
import { FileUploaderComponent } from '../../util/file-uploader/file-uploader.component';
import { CONFIG } from '../../../services/config.service';
import { AccountService } from '../../../services/account.service';
import { MessageFullscreenModalComponent } from '../../util/message-fullscreen-modal/message-fullscreen-modal.component';

@Component({
  selector: 'become-advisor',
  templateUrl: './become-advisor.component.html',
  styleUrls: ['./become-advisor.component.css']
})
export class BecomeAdvisorComponent implements ModalComponent, OnInit {
  @Input() data: any;
  @Output() setClose = new EventEmitter<void>();
  @Output() setNewModal = new EventEmitter<FullscreenModalComponentInput>();
  
  requestToBeAdvisorRequest: RequestToBeAdvisorRequest = new RequestToBeAdvisorRequest();
  requestForm: FormGroup;
  promise: Subscription;
  pictureUrl: string;
  confirmedPassword: string;
  invalidPassword: boolean;
  acceptTermsAndConditions: boolean;
  @ViewChild("RecaptchaComponent") RecaptchaComponent: RecaptchaComponent;
  @ViewChild("FileUploadComponent") FileUploadComponent: FileUploaderComponent;

  constructor(private formBuilder: FormBuilder,
    private advisorService: AdvisorService, 
    private accountService: AccountService,
    private notificationsService: NotificationsService) { 
    this.buildForm();
  }

  private buildForm() {
    if (this.isNewUser()) {
      this.requestForm = this.formBuilder.group({
        email: ['', Validators.compose([Validators.required, Validators.maxLength(50)])],
        password: ['', Validators.compose([Validators.required, Validators.maxLength(100), Validators.minLength(8)])],
        name: ['', Validators.compose([Validators.required, Validators.maxLength(50)])],
        description: ['', Validators.compose([Validators.required, Validators.maxLength(160)])],
        experience: ['', Validators.compose([Validators.required, Validators.maxLength(4000)])]
      });
    } else {
      this.requestForm = this.formBuilder.group({
        name: ['', Validators.compose([Validators.required, Validators.maxLength(50)])],
        description: ['', Validators.compose([Validators.required, Validators.maxLength(160)])],
        experience: ['', Validators.compose([Validators.required, Validators.maxLength(4000)])]
      });
    }
  }

  ngOnInit() {
    this.advisorService.getRequestToBeAdvisor().subscribe(result => 
      {
        this.acceptTermsAndConditions = false;
        this.invalidPassword = false;
        this.requestToBeAdvisorRequest.email = "";
        this.requestToBeAdvisorRequest.password = "";
        let currentRequestToBeAdvisor: RequestToBeAdvisor = result;
        if(!!currentRequestToBeAdvisor){
          this.requestToBeAdvisorRequest.name = currentRequestToBeAdvisor.name;
          this.requestToBeAdvisorRequest.description = currentRequestToBeAdvisor.description;
          this.requestToBeAdvisorRequest.previousExperience = currentRequestToBeAdvisor.previousExperience;
          if (!!currentRequestToBeAdvisor.urlGuid) {
            this.FileUploadComponent.forceImageUrl(CONFIG.profileImgUrl.replace("{id}", currentRequestToBeAdvisor.urlGuid));
          }
        }
      });
  }

  isNewUser() {
    return !this.accountService.isLoggedIn();
  }

  public onCaptchaResponse(captchaResponse: string) {
    this.requestToBeAdvisorRequest.captcha = captchaResponse;
  }

  onChangePassword() {
    this.invalidPassword = !!this.confirmedPassword && this.confirmedPassword !== this.requestToBeAdvisorRequest.password;
  }

  onSubmit() {
    if (!this.acceptTermsAndConditions) {
      this.notificationsService.error(null, "You must accept the terms and conditions to continue.");
    } else if (this.isNewUser() && this.confirmedPassword !== this.requestToBeAdvisorRequest.password) {
      this.notificationsService.error(null, "Passwords do not match.");
    } else if (!this.requestToBeAdvisorRequest.captcha) {
      this.notificationsService.error(null, "Fill the captcha.");
    } else if (this.requestForm.valid) {
      this.requestToBeAdvisorRequest.changedPicture = this.FileUploadComponent.fileWasChanged();
      this.requestToBeAdvisorRequest.file = this.FileUploadComponent.getFile();
      this.advisorService.postRequestToBeAdvisor(this.requestToBeAdvisorRequest).subscribe(result => 
      {
        let modalData = new FullscreenModalComponentInput();
        modalData.component = MessageFullscreenModalComponent;
        modalData.componentInput = { message: "Request was successfully sent.", redirectUrl: "" };
        modalData.title = "";
        this.setNewModal.emit(modalData);
      }, this.RecaptchaComponent.reset);
    }
  }

  canSubmit() {
    return this.acceptTermsAndConditions && !this.invalidPassword && !!this.requestToBeAdvisorRequest.captcha && this.requestForm.valid;
  }

  getRequestNameLength() {
    if (!!this.requestToBeAdvisorRequest.name) {
      return this.requestToBeAdvisorRequest.name.length;
    } else {
      return 0;
    }
  }

  getRequestDescriptionLength() {
    if (!!this.requestToBeAdvisorRequest.description) {
      return this.requestToBeAdvisorRequest.description.length;
    } else {
      return 0;
    }
  }

  getRequestEmailLength() {
    if (!!this.requestToBeAdvisorRequest.email) {
      return this.requestToBeAdvisorRequest.email.length;
    } else {
      return 0;
    }
  }
}
