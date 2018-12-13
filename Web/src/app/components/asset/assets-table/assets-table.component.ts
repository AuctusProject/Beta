import { Component, OnInit, Input, ViewChild, SimpleChanges } from '@angular/core';
import { AssetResponse } from '../../../model/asset/assetResponse';
import { FollowUnfollowType } from '../../util/follow-unfollow/follow-unfollow.component';
import { MatSort, MatTableDataSource, MatPaginator } from '@angular/material';
import { Util } from '../../../util/util';

@Component({
  selector: 'assets-table',
  templateUrl: './assets-table.component.html',
  styleUrls: ['./assets-table.component.css']
})
export class AssetsTableComponent implements OnInit {
  @Input() assets : AssetResponse[];
  @Input() showViewMore : boolean;

  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  dataSource= new MatTableDataSource<AssetResponse>(this.assets);

  displayedColumns: string[] = [
    "market",
    "sentiment",
    "marketCap",
    "lastPrice",
    "change",
    "trade"
  ];
  expertFollowUnfollowType = FollowUnfollowType.asset;

  constructor() { }

  ngOnInit() {
  }

  ngOnChanges(changes: SimpleChanges) {
    if(changes.assets.previousValue != changes.assets.currentValue){
      this.setDataSource();
    }
  }
  
  setDataSource(){
    this.dataSource.data = this.assets;
    if(this.dataSource.data != null && this.dataSource.data.length > 0){
      if(!this.dataSource.sort){
        this.dataSource.sortingDataAccessor = Util.CustomSortingData;
        this.dataSource.sort = this.sort;
      }
      if(!this.dataSource.paginator){
        this.dataSource.paginator = this.paginator;
      }
    }
  }

  onNewVariation24h($event, asset: AssetResponse){
    asset.variation24h = $event;
  }

  onNewLastValue($event, asset: AssetResponse){
    asset.lastValue = $event;
  }

  onNewMarketCap($event, asset: AssetResponse){
    asset.marketCap = $event;
  }
}
