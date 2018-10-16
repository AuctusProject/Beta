import { Component, OnInit, Input, OnChanges } from '@angular/core';
import { AssetAdvisorResponse } from 'src/app/model/asset/assetResponse';
import { AssetService } from 'src/app/services/asset.service';
import { AssetRatingsResponse } from 'src/app/model/asset/assetRatingsResponse';

@Component({
  selector: 'ratings-list',
  templateUrl: './ratings-list.component.html',
  styleUrls: ['./ratings-list.component.css']
})
export class RatingsListComponent implements OnInit, OnChanges {
  @Input() assetId: number;
  displayedColumns: string[] = ['time','recommendation','target','stopLoss','expert','rating'];
  ratings: AssetRatingsResponse[];
  constructor(private assetService: AssetService) { }

  ngOnInit() {
  }

  ngOnChanges() {
    this.loadRatings();
  }

  loadRatings(){
    this.assetService.getAssetRatings(this.assetId).subscribe(result => {
      this.ratings = result;
    });
  }
}
