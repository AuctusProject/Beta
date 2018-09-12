import { Component, OnInit } from '@angular/core';
import { AssetService } from '../../../services/asset.service';
import { AssetResponse } from '../../../model/asset/assetResponse';
import { FullscreenModalComponentInput } from '../../../model/modal/fullscreenModalComponentInput';
import { NewAdviceComponent } from '../../advisor/new-advice/new-advice.component';
import { FullscreenModalComponent } from '../../util/fullscreen-modal/fullscreen-modal.component';
import { MatDialog } from '@angular/material';
import { AccountService } from '../../../services/account.service';

@Component({
  selector: 'list-assets',
  templateUrl: './list-assets.component.html',
  styleUrls: ['./list-assets.component.css']
})
export class ListAssetsComponent implements OnInit {
  allAssets : AssetResponse[] = [];
  assets : AssetResponse[] = [];
  showNewAdviceButton: boolean = false;

  constructor(public dialog: MatDialog, 
    public accountService: AccountService, 
    private assetService: AssetService) { }

  currentPage = 1;
  pageSize = 6;

  ngOnInit() {
    this.showNewAdviceButton = this.accountService.isLoggedIn() && this.accountService.getLoginData().isAdvisor;
    this.assetService.getAssetsDetails().subscribe(result => 
      {
        this.allAssets = result;
        this.setVisibleAssets();
      });
  }

  onNewAdviceClick() {
    let modalData = new FullscreenModalComponentInput();
    modalData.component = NewAdviceComponent;
    this.dialog.open(FullscreenModalComponent, { data: modalData }); 
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
