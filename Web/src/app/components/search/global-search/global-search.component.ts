import { Component, OnInit } from '@angular/core';
import { Observable } from '../../../../../node_modules/rxjs';
import { Asset } from '../../../model/asset/asset';
import { AccountService } from '../../../services/account.service';
import { AdvisorResponse } from '../../../model/advisor/advisorResponse';

@Component({
  selector: 'global-search',
  templateUrl: './global-search.component.html',
  styleUrls: ['./global-search.component.css']
})
export class GlobalSearchComponent implements OnInit {
  assetResults: Observable<Asset[]>;
  expertResults: Observable<AdvisorResponse[]>;
  constructor(private accountService: AccountService) { }

  ngOnInit() {
  }

  onInputChanged(searchStr: string): void {
    this.assetResults = new Observable<Asset[]>();
    this.accountService.search(searchStr).subscribe(result => {
      this.assetResults = result.assets;
      this.expertResults = result.advisors;
    });
  }

}
