import { Component, OnInit, ViewChild } from '@angular/core';
import { CONFIG } from '../../../services/config.service';
import { MatMenu } from '@angular/material';
import { ModalService } from '../../../services/modal.service';

@Component({
  selector: 'home-header',
  templateUrl: './home-header.component.html',
  styleUrls: ['./home-header.component.css']
})
export class HomeHeaderComponent implements OnInit {
  menuOpen: boolean = false;
  @ViewChild("mobile") mobile: MatMenu;
  public constructor(private modalService: ModalService) { }

  ngOnInit() {
    if (!!this.mobile) {
      this.mobile.closed.subscribe(() => this.menuOpen = false);
    }
  }

  signup(){
    this.modalService.setRegister();
  }

  login(){
    this.modalService.setLogin();
  }

  getLogoImgUrl() {
    return CONFIG.platformImgUrl.replace("{id}", "logo_trading");
  }
}
