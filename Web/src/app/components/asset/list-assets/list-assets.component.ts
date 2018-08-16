import { Component, OnInit } from '@angular/core';
import { AssetService } from '../../../services/asset.service';
import { AssetResponse } from '../../../model/asset/assetResponse';

@Component({
  selector: 'list-assets',
  templateUrl: './list-assets.component.html',
  styleUrls: ['./list-assets.component.css']
})
export class ListAssetsComponent implements OnInit {
  assets : AssetResponse[] = [];
  constructor(private assetService: AssetService) { }

  ngOnInit() {
    this.assetService.getAssetsDetails().subscribe(result => this.assets = result);
  }
  

}
