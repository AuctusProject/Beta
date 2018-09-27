import { Component, OnInit, EventEmitter, Input, Output, ViewChild } from '@angular/core';
import { RequestToBeAdvisor } from '../../../model/advisor/requestToBeAdvisor';
import { AdvisorService } from '../../../services/advisor.service';
import { RequestToBeAdvisorRequest } from '../../../model/advisor/requestToBeAdvisorRequest';
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

@Component({
  selector: 'become-advisor',
  templateUrl: './become-advisor.component.html',
  styleUrls: ['./become-advisor.component.css']
})
export class BecomeAdvisorComponent implements ModalComponent, OnInit {
  modalTitle: string = "Become an expert";
  @Input() data: any;
  @Output() setClose = new EventEmitter<void>();
  @Output() setNewModal = new EventEmitter<FullscreenModalComponentInput>();
  
  requestToBeAdvisorRequest: RequestToBeAdvisorRequest = new RequestToBeAdvisorRequest();
  promise: Subscription;
  @ViewChild("RecaptchaComponent") RecaptchaComponent: RecaptchaComponent;
  @ViewChild("FileUploadComponent") FileUploadComponent: FileUploaderComponent;
  @ViewChild("Name") Name: InheritanceInputComponent;
  @ViewChild("Email") Email: InheritanceInputComponent;
  @ViewChild("Password") Password: InheritanceInputComponent;
  @ViewChild("Description") Description: InheritanceInputComponent;
  @ViewChild("Experience") Experience: InheritanceInputComponent;
  
  alreadySent: boolean = false;
  requestDenied: boolean = false;

  constructor(private advisorService: AdvisorService, 
    private accountService: AccountService,
    private notificationsService: NotificationsService,
    private authRedirect: AuthRedirect) { }

  ngOnInit() {
    if (this.accountService.getLoginData().isAdvisor) {
      this.setClose.emit();
      this.authRedirect.redirectAfterLoginAction();
    } else {
      this.advisorService.getRequestToBeAdvisor().subscribe(result => 
        {
          this.requestToBeAdvisorRequest.email = this.isNewUser() ? "" : this.accountService.getLoginData().email;
          this.requestToBeAdvisorRequest.password = "";
          let currentRequestToBeAdvisor: RequestToBeAdvisor = result;
          if(!!currentRequestToBeAdvisor){
            if (currentRequestToBeAdvisor.approved == true) {
              this.setClose.emit();
              this.authRedirect.redirectAfterLoginAction();
            } else {
              this.requestDenied = currentRequestToBeAdvisor.approved == false;
              this.alreadySent = true;
              this.requestToBeAdvisorRequest.name = currentRequestToBeAdvisor.name;
              this.requestToBeAdvisorRequest.description = currentRequestToBeAdvisor.description;
              this.requestToBeAdvisorRequest.previousExperience = currentRequestToBeAdvisor.previousExperience;
              if (!!currentRequestToBeAdvisor.urlGuid) {
                this.FileUploadComponent.forceImageUrl(CONFIG.profileImgUrl.replace("{id}", currentRequestToBeAdvisor.urlGuid));
              }
            }
          }
        });
    }
  }

  isNewUser() {
    return !this.accountService.isLoggedIn();
  }

  public onCaptchaResponse(captchaResponse: string) {
    this.requestToBeAdvisorRequest.captcha = captchaResponse;
  }

  onSubmit() {
    if (this.isNewUser() && !this.requestToBeAdvisorRequest.captcha) {
      this.notificationsService.error(null, "You must fill the captcha.");
    } else if (this.isValidRequest()) {
      this.requestToBeAdvisorRequest.changedPicture = this.FileUploadComponent.fileWasChanged();
      this.requestToBeAdvisorRequest.file = this.FileUploadComponent.getFile();
      this.promise = this.advisorService.postRequestToBeAdvisor(this.requestToBeAdvisorRequest).subscribe(result => 
      {
        if (!!result && !result.error && result.data) {
          this.accountService.setLoginData(result.data);
          let modalData = new FullscreenModalComponentInput();
          modalData.component = MessageFullscreenModalComponent;
          modalData.componentInput = { message: "Thank you for submitting your details. An Auctus representative will be in touch shortly.", redirectUrl: "" };
          this.setNewModal.emit(modalData);
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
    isValid = this.Experience.isValid() && isValid;
    if (this.isNewUser()) {
      isValid = this.Email.isValid() && isValid;
      isValid = this.Password.isValid() && isValid;
    }
    return isValid;
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
    return { textOptions: { placeHolder: "Short description", maxLength: 160 } };
  }

  getExperienceOptions() {
    return { inputType: InputType.TextArea, textOptions: { placeHolder: "Describe your expertise" }, textAreaOptions: { maxRows: 20, minRows: 5 } };
  }
}
