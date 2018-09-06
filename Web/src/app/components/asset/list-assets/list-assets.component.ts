import { Component, OnInit } from '@angular/core';
import { AssetService } from '../../../services/asset.service';
import { AssetResponse } from '../../../model/asset/assetResponse';

@Component({
  selector: 'list-assets',
  templateUrl: './list-assets.component.html',
  styleUrls: ['./list-assets.component.css']
})
export class ListAssetsComponent implements OnInit {
  allAssets : AssetResponse[] = [];
  assets : AssetResponse[] = [];
  constructor(private assetService: AssetService) { }

  currentPage = 1;
  pageSize = 6;

  ngOnInit() {
    this.assetService.getAssetsDetails().subscribe(result => 
      {
        this.allAssets = result;
        this.setVisibleAssets();
      });
  }
  
  loadMoreAssets(){
    this.currentPage++;
    this.setVisibleAssets();
  }

  hasMoreAssets(){
    return this.allAssets != null && this.assets.length != this.allAssets.length;
  }

  setVisibleAssets(){
    var numberOfAssetsToShow = this.pageSize * this.currentPage;
    this.assets = this.allAssets.slice(0, numberOfAssetsToShow);
  }
}
