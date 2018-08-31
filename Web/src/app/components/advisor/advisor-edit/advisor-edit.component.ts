import { Component, OnInit, ViewChild } from '@angular/core';
import { AdvisorService } from '../../../services/advisor.service';
import { ActivatedRoute } from '../../../../../node_modules/@angular/router';
import { AdvisorRequest } from '../../../model/advisor/advisorRequest';
import { Subscription } from '../../../../../node_modules/rxjs';
import { AccountService } from '../../../services/account.service';
import { NavigationService } from '../../../services/navigation.service';
import { Advisor } from '../../../model/advisor/advisor';
import { FileUploaderComponent } from '../../util/file-uploader/file-uploader.component';
import { CONFIG } from "../../../services/config.service";
import { NotificationsService } from 'angular2-notifications';

@Component({
  selector: 'advisor-edit',
  templateUrl: './advisor-edit.component.html',
  styleUrls: ['./advisor-edit.component.css']
})
export class AdvisorEditComponent implements OnInit {
  advisor: Advisor;
  promise: Subscription;
  emptyUserUrl: string;
  executing: boolean;
  @ViewChild("FileUploadComponent") FileUploadComponent: FileUploaderComponent;

  constructor(private route: ActivatedRoute, private advisorService: AdvisorService, 
    private accountService: AccountService, private navigationService: NavigationService,
    private notificationsService: NotificationsService) { }

  ngOnInit() {
    this.route.params.subscribe(params => {
        if(this.accountService.getLoginData().isAdvisor){
          if(this.accountService.getLoginData().id == params['id']){
            this.advisorService.getAdvisor(params['id']).subscribe(advisor => 
              {
                this.emptyUserUrl = CONFIG.profileImgUrl.replace("{id}", params['id']);
                this.advisor = advisor;
              });
          }
          else{
            this.navigationService.goToAdvisorDetails(params['id']);
          }
        }
        else{
          this.navigationService.goToFeed();
        }
      }
    );
  }

  save() {
    var request = new AdvisorRequest();
    request.name = this.advisor.name;
    request.description = this.advisor.description;
    request.changedPicture = this.FileUploadComponent.fileWasChanged();
    request.file = this.FileUploadComponent.getFile();
    this.promise = this.advisorService.editAdvisor(this.advisor.id, request).subscribe(result => 
      {
        this.emptyUserUrl = CONFIG.profileImgUrl.replace("{id}", result);
        this.notificationsService.success(null, "Successfully saved.");
      });
  }

  back() {
    this.navigationService.goToFeed();
  }
}
