import { Component, OnInit } from '@angular/core';
import { CONFIG } from '../../../services/config.service';

@Component({
  selector: 'be-an-expert',
  templateUrl: './be-an-expert.component.html',
  styleUrls: ['./be-an-expert.component.css']
})
export class BeAnExpertComponent implements OnInit {

  constructor() { }

  ngOnInit() {
  }

  getLogoImgUrl() {
    return CONFIG.platformImgUrl.replace("{id}", "logo_black");
  }

  getEmailInputOptions() {
    return { textOptions: { outlineField: false, placeHolder: "Email", required: false, showHintSize: false }, darkLayout:true };
  }
  
  getNameInputOptions() {
    return { textOptions: { outlineField: false, placeHolder: "Name", required: false, showHintSize: false }, darkLayout:true };
  }

  getTwitterInputOptions() {
    return { textOptions: { outlineField: false, placeHolder: "Twitter", required: false, showHintSize: false }, darkLayout:true };
  }
}
