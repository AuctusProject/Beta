import { Component, OnInit, Input } from '@angular/core';

@Component({
  selector: 'top-image',
  templateUrl: './top-image.component.html',
  styleUrls: ['./top-image.component.css']
})
export class TopImageComponent implements OnInit {
  @Input() urlImage: string; 
  @Input() title: string;
  @Input() subtitle: string;
  @Input() subtitle2: string;
  @Input() showLatestUpdates: boolean = false;
  @Input() latestUpdates: string[];

  public constructor() { }

  ngOnInit() {
  }
}
