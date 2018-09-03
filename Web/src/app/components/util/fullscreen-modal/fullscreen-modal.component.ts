import { Component, OnInit, Input, ViewChild, Inject, ComponentFactoryResolver } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { ModalDirective } from '../../../directives/modal.directive';
import { ModalComponent } from '../../../model/modal/modalComponent';

@Component({
  selector: 'fullscreen-modal',
  templateUrl: './fullscreen-modal.component.html',
  styleUrls: ['./fullscreen-modal.component.css']
})
export class FullscreenModalComponent implements OnInit {
  @ViewChild(ModalDirective) modalHost: ModalDirective;

  constructor(private componentFactoryResolver: ComponentFactoryResolver,
    public dialogRef: MatDialogRef<FullscreenModalComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any) { 
  }

  ngOnInit() {
    this.loadContentComponent();
  }

  loadContentComponent() {
    let componentFactory = this.componentFactoryResolver.resolveComponentFactory(this.data.component);
    let viewContainerRef = this.modalHost.viewContainerRef;
    viewContainerRef.clear();
    let componentRef = viewContainerRef.createComponent(componentFactory);
    (<ModalComponent>componentRef.instance).data = this.data.input;
  }
}
