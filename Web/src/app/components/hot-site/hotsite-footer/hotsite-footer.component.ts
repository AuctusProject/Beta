import { Component, OnInit } from '@angular/core';
import { CONFIG } from '../../../services/config.service';

@Component({
  selector: 'hotsite-footer',
  templateUrl: './hotsite-footer.component.html',
  styleUrls: ['./hotsite-footer.component.css']
})
export class HotsiteFooterComponent implements OnInit {

  constructor() { }

  ngOnInit() {
  }

  getLogoImgUrl() {
    return CONFIG.platformImgUrl.replace("{id}", "logo_black");
  }
}
