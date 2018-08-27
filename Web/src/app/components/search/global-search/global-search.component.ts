import { Component, OnInit, NgZone } from '@angular/core';
import { AccountService } from '../../../services/account.service';
import { SearchResponse } from '../../../model/search/searchResponse';
import { MatAutocompleteSelectedEvent } from '@angular/material';
import { Router } from '@angular/router';

@Component({
  selector: 'global-search',
  templateUrl: './global-search.component.html',
  styleUrls: ['./global-search.component.css']
})
export class GlobalSearchComponent implements OnInit {
  searchResults: SearchResponse = new SearchResponse();
  timer: any;
  inputText: string;
  constructor(private accountService: AccountService,
    private router: Router, 
    private zone : NgZone) { }

  ngOnInit() {
  }

  onInputChanged(searchStr: string): void {
    if (searchStr && searchStr.length >= 3) {
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
    });
  }

  onSelectionChanged(event: MatAutocompleteSelectedEvent) {
    if (event.option.value) {
      this.inputText = event.option.value.name;
      if (event.option.value.code) {
        this.zone.run(() => this.router.navigate(['asset-details', event.option.value.id]));
      } else {
        this.zone.run(() => this.router.navigate(['advisor-details', event.option.value.id]));
      }
    } else {
      this.inputText = "";
    }
  }
}
