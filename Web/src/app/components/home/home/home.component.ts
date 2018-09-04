import { Component, OnInit } from '@angular/core';
import { LoginComponent } from '../../account/login/login.component';
import { MatDialog } from '@angular/material';
import { FullscreenModalComponent } from '../../util/fullscreen-modal/fullscreen-modal.component';
import { FullscreenModalComponentInput } from '../../../model/modal/fullscreenModalComponentInput';

@Component({
  selector: 'home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {

  constructor(public dialog: MatDialog) { }

  ngOnInit() {
  }

  onLogin(){
    let loginModalData = new FullscreenModalComponentInput();
    loginModalData.component = LoginComponent;
    loginModalData.title = "Login";
    this.dialog.open(FullscreenModalComponent, { data: loginModalData }); 
  }
}
