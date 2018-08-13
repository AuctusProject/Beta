import { Component, OnInit, Input } from '@angular/core';
import { AssetResponse } from '../../../model/asset/assetResponse';

@Component({
  selector: 'asset-card',
  templateUrl: './asset-card.component.html',
  styleUrls: ['./asset-card.component.css']
})
export class AssetCardComponent implements OnInit {
  @Input() asset: AssetResponse;
  constructor() { }

  ngOnInit() {
  }

}
