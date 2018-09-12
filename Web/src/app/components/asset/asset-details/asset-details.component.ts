import { Component, OnInit } from '@angular/core';
import { AssetResponse } from '../../../model/asset/assetResponse';
import { ActivatedRoute } from '../../../../../node_modules/@angular/router';
import { AssetService } from '../../../services/asset.service';
import { Util } from '../../../util/Util';
import { CONFIG} from "../../../services/config.service";
import { AccountService } from '../../../services/account.service';
import { MatDialog } from '@angular/material';
import { FullscreenModalComponentInput } from '../../../model/modal/fullscreenModalComponentInput';
import { NewAdviceComponent } from '../../advisor/new-advice/new-advice.component';
import { FullscreenModalComponent } from '../../util/fullscreen-modal/fullscreen-modal.component';

@Component({
  selector: 'asset-details',
  templateUrl: './asset-details.component.html',
  styleUrls: ['./asset-details.component.css']
})
export class AssetDetailsComponent implements OnInit {
  asset: AssetResponse;
  showNewAdviceButton: boolean = false;

  constructor(private route: ActivatedRoute, 
    private assetService: AssetService,
    public dialog: MatDialog, 
    public accountService: AccountService) { }

  ngOnInit() {
    this.showNewAdviceButton = this.accountService.isLoggedIn() && this.accountService.getLoginData().isAdvisor;
    this.route.params.subscribe(params => 
      this.assetService.getAssetDetails(params['id']).subscribe(asset => this.asset = asset)
    )
  }

  onNewAdviceClick() {
    let modalData = new FullscreenModalComponentInput();
    modalData.component = NewAdviceComponent;
    modalData.componentInput = { assetId: this.asset.assetId };
    this.dialog.open(FullscreenModalComponent, { data: modalData }); 
  }

  getAssetImgUrl(asset: AssetResponse){
    return CONFIG.assetImgUrl.replace("{id}", asset.assetId.toString());
  }

  getRecommendationFromType(type: number){
    return Util.GetRecommendationTypeDescription(type);
  }
  
}
