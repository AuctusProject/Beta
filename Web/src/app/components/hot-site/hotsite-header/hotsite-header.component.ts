import { Component, OnInit, ViewChild } from '@angular/core';
import { CONFIG } from '../../../services/config.service';
import { MatMenu } from '@angular/material';

@Component({
  selector: 'hotsite-header',
  templateUrl: './hotsite-header.component.html',
  styleUrls: ['./hotsite-header.component.css']
})
export class HotsiteHeaderComponent implements OnInit {
  menuOpen: boolean = false;
  @ViewChild("mobile") mobile: MatMenu;
  constructor() { }

  ngOnInit() {
    if (!!this.mobile) {
      this.mobile.closed.subscribe(() => this.menuOpen = false);
    }
  }

  getLogoImgUrl() {
    return CONFIG.platformImgUrl.replace("{id}", "logo_black");
  }
}
