import { Component, OnInit } from '@angular/core';
import { CONFIG } from '../../../services/config.service';

@Component({
  selector: 'footer',
  templateUrl: './footer.component.html',
  styleUrls: ['./footer.component.css']
})
export class FooterComponent implements OnInit {

  constructor() { }

  ngOnInit() {
  }

  getLogoImgUrl() {
    return CONFIG.platformImgUrl.replace("{id}", "logo_black");
  }
}
