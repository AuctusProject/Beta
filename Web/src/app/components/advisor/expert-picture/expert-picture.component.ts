import { Component, OnInit, Input } from '@angular/core';
import { CONFIG } from '../../../services/config.service';

@Component({
  selector: 'expert-picture',
  templateUrl: './expert-picture.component.html',
  styleUrls: ['./expert-picture.component.css']
})
export class ExpertPictureComponent implements OnInit {
  @Input() urlGuid : string;
  @Input() size?: string;
  @Input() ranking?: number;
  constructor() { }

  ngOnInit() {
  }
  
  getAdvisorImgUrl(){
    return CONFIG.profileImgUrl.replace("{id}", this.urlGuid);
  }
}
