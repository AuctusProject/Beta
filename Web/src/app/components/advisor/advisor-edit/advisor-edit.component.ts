import { Component, OnInit, ViewChild, EventEmitter, Input, Output } from '@angular/core';
import { AdvisorService } from '../../../services/advisor.service';
import { ActivatedRoute } from '@angular/router';
import { AdvisorRequest } from '../../../model/advisor/advisorRequest';
import { Subscription } from 'rxjs';
import { AccountService } from '../../../services/account.service';
import { NavigationService } from '../../../services/navigation.service';
import { Advisor } from '../../../model/advisor/advisor';
import { FileUploaderComponent } from '../../util/file-uploader/file-uploader.component';
import { CONFIG } from "../../../services/config.service";
import { FullscreenModalComponentInput } from '../../../model/modal/fullscreenModalComponentInput';
import { ModalComponent } from '../../../model/modal/modalComponent';
import { InheritanceInputComponent } from '../../util/inheritance-input/inheritance-input.component';
import { InputType } from '../../../model/inheritanceInputOptions';
import { MessageFullscreenModalComponent } from '../../util/message-fullscreen-modal/message-fullscreen-modal.component';

@Component({
  selector: 'advisor-edit',
  templateUrl: './advisor-edit.component.html',
  styleUrls: ['./advisor-edit.component.css']
})
export class AdvisorEditComponent implements ModalComponent, OnInit {
  modalTitle: string = "Edit profile";
  @Input() data: any;
  @Output() setClose = new EventEmitter<void>();
  @Output() setNewModal = new EventEmitter<FullscreenModalComponentInput>();
  
  advisor: Advisor = new Advisor();
  promise: Subscription;
  @ViewChild("FileUploadComponent") FileUploadComponent: FileUploaderComponent;  
  @ViewChild("Name") Name: InheritanceInputComponent;
  @ViewChild("Description") Description: InheritanceInputComponent;

  constructor(private route: ActivatedRoute, private advisorService: AdvisorService, 
    private accountService: AccountService, private navigationService: NavigationService) { }

  ngOnInit() {
    if (this.accountService.getLoginData().isAdvisor && !!this.data && !!this.data.id) {
      if (this.accountService.getLoginData().id == this.data.id){
        this.advisorService.getAdvisor(this.data.id).subscribe(advisor => 
          {
            this.FileUploadComponent.forceImageUrl(CONFIG.profileImgUrl.replace("{id}", advisor.urlGuid));
            this.advisor = advisor;
          });
      } else {
        this.setClose.emit();
        this.navigationService.goToExpertDetails(this.data.id);
      }
    } else {
      this.setClose.emit();
      this.navigationService.goToFeed();
    }
  }

  save() {
    if (this.isValidRequest()) {
      var request = new AdvisorRequest();
      request.name = this.advisor.name;
      request.description = this.advisor.description;
      request.changedPicture = this.FileUploadComponent.fileWasChanged();
      request.file = this.FileUploadComponent.getFile();
      this.promise = this.advisorService.editAdvisor(this.advisor.id, request).subscribe(result => 
        {
          let modalData = new FullscreenModalComponentInput();
          modalData.hiddenClose = true;
          modalData.component = MessageFullscreenModalComponent;
          modalData.componentInput = { message: "Your profile has been updated successfully.", reload: true };
          this.setNewModal.emit(modalData);
        });
    }
  }

  isValidRequest() : boolean {
    let isValid = this.Name.isValid();
    return this.Description.isValid() && isValid;
  }

  getNameOptions() {
    return { textOptions: { placeHolder: "Name", browserAutocomplete: "name", maxLength: 50 } };
  }

  getDescriptionOptions() {
    return { inputType: InputType.TextArea, textOptions: { placeHolder: "Short description", maxLength: 160 } };
  }
}
