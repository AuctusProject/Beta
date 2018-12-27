import { Component, OnInit, ViewChild } from '@angular/core';
import { Title, Meta } from '@angular/platform-browser';
import { AssetResponse } from '../../../model/asset/assetResponse';
import { ActivatedRoute } from '@angular/router';
import { AssetService } from '../../../services/asset.service';
import { CONFIG} from "../../../services/config.service";
import { FollowUnfollowType } from '../../util/follow-unfollow/follow-unfollow.component';
import { ModalService } from '../../../services/modal.service';
import { AccountService } from '../../../services/account.service';
import { OrderPositionTabComponent } from '../../trade/order-position-tab/order-position-tab.component';
import { ValueDisplayPipe } from '../../../util/value-display.pipe';

@Component({
  selector: 'asset-details',
  templateUrl: './asset-details.component.html',
  styleUrls: ['./asset-details.component.css']
})
export class AssetDetailsComponent implements OnInit {
  asset: AssetResponse;
  assetFollowUnfollowType = FollowUnfollowType.asset;
  logged: boolean = false;
  @ViewChild("OrderTab") OrderTab: OrderPositionTabComponent;

  constructor(private route: ActivatedRoute, 
    private assetService: AssetService,
    private modalService: ModalService,
    private accountService: AccountService,
    private titleService: Title,
    private metaTagService: Meta) { }

  ngOnInit() {
    this.route.params.subscribe(params => 
      this.assetService.getAssetDetails(params['id']).subscribe(
        asset => {
          this.asset = asset;
          this.setTitle(asset);
          this.metaTagService.updateTag({name: 'description', content: "Traders on " + asset.name + " (" + asset.code + ")" });
        })
    );
    this.logged = this.accountService.isLoggedIn();
  }

  getAssetImgUrl() {
    return CONFIG.assetImgUrl.replace("{id}", this.asset.assetId.toString());
  }

  onNewTrade(asset: AssetResponse) {
    this.modalService.setNewOrderDialog(asset);
  }

  showCancelOpenOrders() {
    return this.OrderTab && this.OrderTab.showCancelOpenOrders();
  }

  showCloseOpenPositions() {
    return this.OrderTab && this.OrderTab.showCloseOpenPositions();
  }

  closeAllOpenPositions(assetId: number, assetCode: string) {
    if (this.OrderTab) {
      this.OrderTab.closeAllOpenPositions(assetId, assetCode);
    }
  }

  cancelAllOpenOrders() {
    if (this.OrderTab) {
      this.OrderTab.cancelAllOpenOrders();
    }
  }

  onNewLastValue($event, asset: AssetResponse) {
    asset.lastValue = $event;
    this.setTitle(asset);
  }

  setTitle(asset: AssetResponse) {
    this.titleService.setTitle(new ValueDisplayPipe().transform(asset.lastValue, "$")  + " " + asset.code + " - Auctus Trading");
  }

  showChartPriceAlert() {
    return this.asset && this.asset.pair && this.asset.pair.multipliedSymbol;
  }
}