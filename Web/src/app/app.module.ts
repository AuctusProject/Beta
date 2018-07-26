import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { SimpleNotificationsModule } from 'angular2-notifications';
import { ChartModule } from 'angular-highcharts';

import { AppComponent } from './app.component';

import { Web3Service } from './services/web3.service';
import { LocalStorageService } from './services/local-storage.service';
import { HttpService } from './services/http.service';

@NgModule({
  declarations: [
    AppComponent
  ],
  imports: [
    BrowserModule,
    SimpleNotificationsModule.forRoot(),
    ChartModule
  ],
  providers: [
    HttpService,
    LocalStorageService,
    Web3Service ],
  bootstrap: [AppComponent]
})
export class AppModule { }
