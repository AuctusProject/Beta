import { Component, OnInit } from '@angular/core';
import { LoginComponent } from '../../account/login/login.component';
import { MatDialog } from '@angular/material';
import { FullscreenModalComponent } from '../../util/fullscreen-modal/fullscreen-modal.component';
import { FullscreenModalComponentInput } from '../../../model/modal/fullscreenModalComponentInput';
import { ActivatedRoute } from '../../../../../node_modules/@angular/router';
import { BecomeAdvisorComponent } from '../../advisor/become-advisor/become-advisor.component';

@Component({
  selector: 'home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {

  constructor(public dialog: MatDialog, private route: ActivatedRoute) { }

  ngOnInit() {
    if (!!this.route.snapshot.queryParams['login']) {
      this.onLogin();
    } else if (!!this.route.snapshot.queryParams['becomeadvisor']) {
      this.onBecomeAdvisor();
    }
  }

  onBecomeAdvisor(){
    let loginModalData = new FullscreenModalComponentInput();
    loginModalData.component = BecomeAdvisorComponent;
    this.dialog.open(FullscreenModalComponent, { data: loginModalData }); 
  }

  onLogin(){
    let loginModalData = new FullscreenModalComponentInput();
    loginModalData.component = LoginComponent;
    this.dialog.open(FullscreenModalComponent, { data: loginModalData }); 
  }
}
