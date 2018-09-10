import { Component, OnInit, Input, Output, EventEmitter, ViewChild } from '@angular/core';
import { AccountService } from '../../../services/account.service';
import { NotificationsService } from 'angular2-notifications';
import { ConfigurationRequest } from '../../../model/account/configurationRequest';
import { ModalComponent } from '../../../model/modal/modalComponent';
import { FullscreenModalComponentInput } from '../../../model/modal/fullscreenModalComponentInput';
import { InputType } from '../../../model/inheritanceInputOptions';
import { Subscription } from 'rxjs';
import { InheritanceInputComponent } from '../../util/inheritance-input/inheritance-input.component';
import { NavigationService } from '../../../services/navigation.service';

@Component({
  selector: 'configuration',
  templateUrl: './configuration.component.html',
  styleUrls: ['./configuration.component.css']
})
export class ConfigurationComponent implements ModalComponent, OnInit {
  modalTitle: string = "Set profile configuration";
  @Input() data: any;
  @Output() setClose = new EventEmitter<void>();
  @Output() setNewModal = new EventEmitter<FullscreenModalComponentInput>();

  configurationRequest: ConfigurationRequest = new ConfigurationRequest();
  wallet: string;
  promise: Subscription;
  @ViewChild("Wallet") Wallet: InheritanceInputComponent;

  constructor(private accountService : AccountService, 
    private notificationsService: NotificationsService,
    private navigationService : NavigationService) { }

  ngOnInit() {
    if (!this.accountService.isLoggedIn()) {
      this.setClose.emit();
      this.navigationService.goToLogin();
    } else {
      this.accountService.getConfiguration().subscribe(result =>
        {
          this.wallet = result.wallet;
          this.configurationRequest.allowNotifications = result.allowNotifications;
        }
      );
    }
  }

  save() {
    this.accountService.setConfiguration(this.configurationRequest).subscribe(result =>
      {
        this.notificationsService.success(null, "Successfully saved.");
        this.setClose.emit();
      }
    );
  }

  getWalletOptions() {
    return { textOptions: { placeHolder: "Registered wallet", showValidatorError: false, showHintSize: false, disabled: true } };
  }
}
