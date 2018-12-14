import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'mini-view',
  templateUrl: './mini-view.component.html',
  styleUrls: ['./mini-view.component.css']
})
export class MiniViewComponent implements OnInit {
  listOnlyWatchlist = false;
  constructor() { }

  ngOnInit() {
  }

  toggleMarketsWatchlist() {
    this.listOnlyWatchlist = !this.listOnlyWatchlist;
  }

  onNewSignal() {

  }
}
