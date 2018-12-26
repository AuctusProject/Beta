import { Component, OnInit, Input } from '@angular/core';

@Component({
  selector: 'menu-item',
  templateUrl: './menu-item.component.html',
  styleUrls: ['./menu-item.component.css']
})
export class MenuItemComponent implements OnInit {
  @Input() link: string;
  @Input() icon: string;
  @Input() name: string;
  @Input() disabled: boolean = false;
  @Input() externalLink: string;
  @Input() noLink: boolean = false;

  constructor() { }

  ngOnInit() {
  }

}
