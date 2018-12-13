import { Component, OnInit, NgZone, ViewChild, ElementRef } from '@angular/core';
import { AccountService } from '../../../services/account.service';
import { SearchResponse } from '../../../model/search/searchResponse';
import { MatAutocompleteSelectedEvent } from '@angular/material';
import { NavigationService } from '../../../services/navigation.service';
import { CONFIG } from '../../../services/config.service';
import { FormControl } from '@angular/forms';

@Component({
  selector: 'global-search',
  templateUrl: './global-search.component.html',
  styleUrls: ['./global-search.component.css']
})
export class GlobalSearchComponent implements OnInit {
  searchResults: SearchResponse = new SearchResponse();
  timer: any;
  inputText: string;
  showExperts: boolean = true;
  showAssets: boolean = true;
  searchControl: FormControl = new FormControl(); 
  @ViewChild('SearchInput') searchInput: ElementRef;
  constructor(private accountService: AccountService,
    private navigationService: NavigationService) { }

  ngOnInit() {
  }

  onInputChanged(searchStr: string): void {
    if (searchStr && searchStr.length > 1) {
      if (this.timer) {
        clearTimeout(this.timer);
      }
      this.timer = setTimeout(() => this.search(searchStr), 700);
    } else {
      this.searchResults = new SearchResponse();
    }
  }

  search(searchStr: string) {
    this.accountService.search(searchStr).subscribe(result => {
      this.searchResults = result;
      this.showAssets = !!result && !!result.assets && result.assets.length > 0;
      this.showExperts = !!result && !!result.advisors && result.advisors.length > 0;
    });
  }

  onSelectionChanged(event: MatAutocompleteSelectedEvent) {
    if (event.option.value) {
      this.searchControl.setValue(event.option.value.name);
      this.inputText = event.option.value.name;
      if (event.option.value.code) {
        this.navigationService.goToAssetDetails(event.option.value.id);
      } else {
        this.navigationService.goToExpertDetails(event.option.value.id);
      }
    } else {
      this.searchControl.setValue("");
      this.inputText = "";
    }
  }
  
  getAssetImgUrl(assetId: number) {
    return CONFIG.assetImgUrl.replace("{id}", assetId.toString());
  }

  getAdvisorImgUrl(guid: string) {
    return CONFIG.profileImgUrl.replace("{id}", guid);
  }

  public focus(){
    if(this.searchInput && this.searchInput.nativeElement){
      this.searchInput.nativeElement.focus();
    }
  }
}
