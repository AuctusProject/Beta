import { Component, OnInit, ViewChild } from '@angular/core';
import { Title, Meta } from '@angular/platform-browser';
import { AssetService } from '../../../services/asset.service';
import { AssetResponse } from '../../../model/asset/assetResponse';
import { AccountService } from '../../../services/account.service';
import { ModalService } from '../../../services/modal.service';
import { CoinSearchComponent } from '../../util/coin-search/coin-search.component';
import { NavigationService } from '../../../services/navigation.service';

@Component({
  selector: 'list-assets',
  templateUrl: './list-assets.component.html',
  styleUrls: ['./list-assets.component.css']
})
export class ListAssetsComponent implements OnInit {
  allAssets : AssetResponse[] = [];
  assets : AssetResponse[] = [];
  showNewAdviceButton: boolean = false;

  @ViewChild("CoinSearch") CoinSearch: CoinSearchComponent;

  constructor(private modalService: ModalService, 
    public accountService: AccountService, 
    private assetService: AssetService,
    private navigationService: NavigationService,
    private titleService: Title,
    private metaTagService: Meta) { }

  currentPage = 1;
  pageSize = 6;

  ngOnInit() {
    this.titleService.setTitle("Auctus Experts - Top Assets");
    this.metaTagService.updateTag({name: 'description', content: "The most popular Cryptocurrencies. Follow your favorite coins and see what analysts are saying about them"});
    this.showNewAdviceButton = this.accountService.isLoggedIn() && this.accountService.getLoginData().isAdvisor;
    this.assetService.getAssetsDetails().subscribe(result => 
      {
        this.allAssets = result;
        this.setVisibleAssets();
      });

      this.CoinSearch.onSelect.subscribe(newValue => 
        {
          if (newValue) {
            this.navigationService.goToAssetDetails(newValue.id);
          }
        }
      );
  }

  onNewAdviceClick() {
    this.modalService.setNewAdvice();
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

  getSearchOptions() {
    return { required: false, outlineField: true, darkStyle: true };
  }
}
