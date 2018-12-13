import { Component, OnInit, Input, OnChanges, OnDestroy, Output, EventEmitter, ChangeDetectorRef, ViewChild } from '@angular/core';
import { AccountService } from '../../../services/account.service';
import { LoginResponse } from '../../../model/account/loginResponse';
import { CONFIG } from '../../../services/config.service';
import { AdvisorResponse } from '../../../model/advisor/advisorResponse';
import { MediaMatcher } from '@angular/cdk/layout';
import { GlobalSearchComponent } from '../global-search/global-search.component';
import { trigger, transition, style, animate } from '@angular/animations';

@Component({
  selector: 'header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.css'],
  animations: [
    trigger(
      'enterAnimation', [
        transition(':enter', [
          style({transform: 'translateX(100%)', opacity: 0}),
          animate('500ms', style({transform: 'translateX(0)', opacity: 1}))
        ]),
        transition(':leave', [
          style({transform: 'translateX(0)', opacity: 1}),
          animate('500ms', style({transform: 'translateX(100%)', opacity: 0}))
        ])
      ]
    ),
    trigger(
      'fadeAnimation', [
        transition(':enter', [
          style({opacity: 0}),
          animate('500ms', style({opacity: 1}))
        ]),
        transition(':leave', [
          style({opacity: 1}),
          animate('500ms', style({opacity: 0}))
        ])
      ]
    )
  ],
})
export class HeaderComponent implements OnInit {
  loginData: LoginResponse;
  @Input() advisor: AdvisorResponse;
  @Output() menuClick = new EventEmitter<void>();
  @Output() searchClick = new EventEmitter<void>();
  searchEnabled=false;
  mobileQuery: MediaQueryList;
  private _mobileQueryListener: () => void;
  @ViewChild('GlobalSearch') globalSearch: GlobalSearchComponent;
  constructor(private accountService : AccountService,
    changeDetectorRef: ChangeDetectorRef, 
    media: MediaMatcher) { 
    this.mobileQuery = media.matchMedia('(max-width: 959px)');
    this._mobileQueryListener = () => changeDetectorRef.detectChanges();
    this.mobileQuery.addListener(this._mobileQueryListener); 
  }
  
  ngOnInit() {
    this.loginData = this.accountService.getLoginData();
  }

  getLogoImgUrl() {
    return CONFIG.platformImgUrl.replace("{id}", "logo_trading");
  }

  isLogged(): boolean {
    this.loginData = this.accountService.getLoginData();
    return !!this.loginData;
  }

  onMenuClick(){
    this.menuClick.emit();
  }

  onSearchClick(){
    this.searchEnabled = !this.searchEnabled;
    this.searchClick.emit();
    var self = this;
    if(this.searchEnabled){
      setTimeout(function(){
        self.globalSearch.focus();
      }, 400);
    }
  }


}
