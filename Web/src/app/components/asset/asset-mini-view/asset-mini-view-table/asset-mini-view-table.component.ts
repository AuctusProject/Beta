import { Component, OnInit, Input, SimpleChanges, ViewChild, Output, EventEmitter } from '@angular/core';
import { AssetResponse } from '../../../../model/asset/assetResponse';
import { MatTableDataSource, MatPaginator } from '@angular/material';
import { FollowUnfollowType } from '../../../../components/util/follow-unfollow/follow-unfollow.component';
import { NavigationService } from '../../../../services/navigation.service';

@Component({
  selector: 'asset-mini-view-table',
  templateUrl: './asset-mini-view-table.component.html',
  styleUrls: ['./asset-mini-view-table.component.css']
})
export class AssetMiniViewTableComponent implements OnInit {
  @Input() assets: AssetResponse[];
  @Output() openMiniChart = new EventEmitter<AssetResponse>();
  @ViewChild(MatPaginator) paginator: MatPaginator;
  dataSource = new MatTableDataSource<AssetResponse>(this.assets);

  displayedColumns: string[] = [
    "market",
    "lastPrice",
    "change",
    "trade"
  ];
  followUnfollowType = FollowUnfollowType.asset;

  constructor(private navigationService: NavigationService) { }

  ngOnInit() {
  }

  ngOnChanges(changes: SimpleChanges) {
    if (changes.assets.previousValue != changes.assets.currentValue) {
      this.assets = changes.assets.currentValue;
      this.setDataSource();
    }
  }
  
  setDataSource(){
    this.dataSource.data = this.assets;
    if (this.dataSource.data != null && this.dataSource.data.length > 0) {
      if(!this.dataSource.paginator) {
        this.dataSource.paginator = this.paginator;
      }
    }
  }

  onNewVariation24h($event, asset: AssetResponse) {
    asset.variation24h = $event;
  }

  onNewLastValue($event, asset: AssetResponse) {
    asset.lastValue = $event;
  }

  goToAsset(assetId: number) {
    this.navigationService.goToAssetDetails(assetId);
  }

  openChart(asset: AssetResponse) {
    this.openMiniChart.emit(asset);
  }
}
