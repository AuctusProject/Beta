import { Component, OnInit } from '@angular/core';
import { CONFIG } from '../../services/config.service';

@Component({
  selector: 'hot-site',
  templateUrl: './hot-site.component.html',
  styleUrls: ['./hot-site.component.css']
})
export class HotSiteComponent implements OnInit {

  constructor() { }

  ngOnInit() {
  }

  getLogoImgUrl() {
    return CONFIG.platformImgUrl.replace("{id}", "logo_black");
  }

  getEmailInputOptions() {
    return { textOptions: { outlineField: false, placeHolder: "Email", required: false, showHintSize: false }, darkLayout:true };
  }

}
