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
  assets : AssetResponse[];

  constructor(private modalService: ModalService, 
    public accountService: AccountService, 
    private assetService: AssetService,
    private titleService: Title,
    private metaTagService: Meta) { }

  ngOnInit() {
    this.titleService.setTitle("Auctus Trading - Markets");
    this.metaTagService.updateTag({name: 'description', content: "The most popular Cryptocurrencies. Follow your favorite coins and see what analysts are saying about them"});
    this.assetService.getAssetsDetails().subscribe(result => 
      {
        this.assets = result;
      });
  }
}
