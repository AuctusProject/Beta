import { Component, OnInit } from '@angular/core';
import { LoginComponent } from '../../account/login/login.component';
import { MatDialog } from '@angular/material';
import { FullscreenModalComponent } from '../../util/fullscreen-modal/fullscreen-modal.component';

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
    const dialogRef = this.dialog.open(FullscreenModalComponent, 
      { data: { component: LoginComponent, title: "Login" } }); 
  }
}
