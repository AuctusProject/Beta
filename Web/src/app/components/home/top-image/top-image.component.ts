import { Component, OnInit } from '@angular/core';
import { ModalService } from '../../../services/modal.service';
import { CONFIG } from '../../../services/config.service';

@Component({
  selector: 'top-image',
  templateUrl: './top-image.component.html',
  styleUrls: ['./top-image.component.css']
})
export class TopImageComponent implements OnInit {

  public constructor(private modalService: ModalService) { }

  ngOnInit() {
  }

  signup(){
    this.modalService.setRegister();
  }

  login(){
    this.modalService.setLogin();
  }

  getTopImage() : string {
    return CONFIG.platformImgUrl.replace("{id}", "home1920px");
  }
}
