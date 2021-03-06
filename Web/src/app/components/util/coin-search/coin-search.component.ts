import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { FormControl, Validators } from '@angular/forms';
import { Asset } from '../../../model/asset/asset';
import { CoinSearchOptions } from '../../../model/search/coinSearchOptions';
import { AssetService } from '../../../services/asset.service';
import { CONFIG } from '../../../services/config.service';
import { Observable } from 'rxjs';
import { startWith, map } from 'rxjs/operators';
import { MatAutocompleteSelectedEvent } from '@angular/material';

@Component({
  selector: 'coin-search',
  templateUrl: './coin-search.component.html',
  styleUrls: ['./coin-search.component.css']
})
export class CoinSearchComponent implements OnInit {
  @Input() options: CoinSearchOptions;
  @Input() assetId: number;
  @Output() onSelect: EventEmitter<Asset> = new EventEmitter<Asset>();

  public required: boolean = false;
  public placeholder: string = "Select an Asset *";
  public outlineField: boolean = true;
  public darkStyle: boolean = true;

  public coinControl: FormControl; 
  public assets: Asset[];
  public searchResults: Asset[] = [];
  public timer: any;
  public inputText: string;
  public assetSelected: Asset;

  public autocompleteClass: string = "search-coin-autocomplete";
  public optionClass: string = "search-coin-autocomplete-option";
  public codeClass: string = "search-coin-code";

  constructor(public assetService: AssetService) { }

  ngOnInit() {
    if (!!this.options) {
        this.required = this.setValue(this.required, this.options.required);
        this.placeholder = "Select an Asset" + (this.required ? ' *' : '');
        this.outlineField = this.setValue(this.outlineField, this.options.outlineField);
        this.darkStyle = this.setValue(this.darkStyle, this.options.darkStyle);
    }

    let validators = [];
    if (this.required) {
      validators.push(Validators.required);
    }
    this.coinControl = new FormControl('', validators);

    this.codeClass += this.darkStyle ? " search-coin-color-dark" : " search-coin-color-light";
    let backgroundLayoutStyle = this.darkStyle ? " search-coin-background-dark" : " search-coin-background-light";
    this.autocompleteClass += backgroundLayoutStyle;
    this.optionClass += backgroundLayoutStyle;

    this.assetService.getAssets().subscribe(result => {
        this.assets = result;
        if (!!this.assetId && this.assetId > 0) {
            this.setForcedCoin(this.assetId);
        }
      });
  }

  public getAssetImgUrl(assetId: number) : string {
    return CONFIG.assetImgUrl.replace("{id}", assetId.toString());
  }

  public setForcedCoin(id: number) {
    if (!!id && id > 0) {
        this.assetId = id;
    }
    if (!!this.assets) {
        let asset = this.assets.filter(option => option.id == this.assetId);
        if (!!asset && asset.length == 1) {
            this.coinControl.setValue(asset[0].name);
            this.inputText = asset[0].name;
            this.setSelected(asset[0]);
        }
    }
  }

  public getSelectedAsset() : Asset {
    this.coinControl.markAsTouched();
    return this.coinControl.valid ? this.coinControl.value : null;
  }

  public setValue(defaultValue: any, optionValue?: any) : any {
    if (optionValue === undefined || optionValue === null) return defaultValue;
    else return optionValue;
  }

  onInputChanged(searchStr: string): void {
    if (searchStr && searchStr.length > 1) {
      if (this.timer) {
        clearTimeout(this.timer);
      }
      this.timer = setTimeout(() =>
      {
        this.setClear();
        const filterValue = searchStr.toLowerCase();
        this.searchResults = this.assets.filter(option => option.name.toLowerCase().indexOf(filterValue) === 0
                || (option.code.toLowerCase().indexOf(filterValue) === 0));
      }, 500);
    } else {
        this.setClear();
        this.searchResults = [];
    }
  }

  public onSelectAsset(selected?: MatAutocompleteSelectedEvent){
    if (selected.option.value) {
        this.coinControl.setValue(selected.option.value.name);
        this.inputText = selected.option.value.name;
        this.setSelected(selected.option.value);
    } else {
        this.inputText = "";
        this.setClear();
    }
  }

  public setSelected(asset: Asset) {
    this.assetSelected = asset;
    this.onSelect.emit(asset);
  }

  public setClear() {
    this.assetSelected = null;
    this.onSelect.emit(null);
  }

  public getErrorMessage() : string {
    if (this.required && (this.coinControl.hasError('required') || !this.assetSelected)) {
        if (this.inputText) {
            this.coinControl.setErrors({'incorrect': true});
        }
        return 'Asset must be selected';
    }
    return '';
  }
}
