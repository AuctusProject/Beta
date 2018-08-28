import { Component, OnInit } from '@angular/core';
import { TopLoadingComponent } from './components/util/top-loading/top-loading.component';


@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit{
  public topLoading = TopLoadingComponent.prototype.constructor;
  constructor(){}

  ngOnInit(){
  }

  public notificationOptions = {
    position: ["bottom", "left"],
    maxStack: 1,
    preventDuplicates: true,
    preventLastDuplicates: "visible",
    clickToClose: true
  }
}