import { Component, OnInit } from '@angular/core';
import { CONFIG } from '../../../services/config.service';

@Component({
  selector: 'hotsite-header',
  templateUrl: './hotsite-header.component.html',
  styleUrls: ['./hotsite-header.component.css']
})
export class HotsiteHeaderComponent implements OnInit {

  constructor() { }

  ngOnInit() {
  }

  getLogoImgUrl() {
    return CONFIG.platformImgUrl.replace("{id}", "logo_black");
  }
}
