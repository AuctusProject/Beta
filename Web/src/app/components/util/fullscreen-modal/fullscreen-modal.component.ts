import { Component, OnInit, OnDestroy, ViewChild, Inject, ComponentFactoryResolver } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { ModalDirective } from '../../../directives/modal.directive';
import { ModalComponent } from '../../../model/modal/modalComponent';
import { FullscreenModalComponentInput } from '../../../model/modal/fullscreenModalComponentInput';

@Component({
  selector: 'fullscreen-modal',
  templateUrl: './fullscreen-modal.component.html',
  styleUrls: ['./fullscreen-modal.component.css']
})
export class FullscreenModalComponent implements OnInit, OnDestroy {
  @ViewChild(ModalDirective) modalHost: ModalDirective;
  title: string;
  private modalComponent: ModalComponent;

  constructor(private componentFactoryResolver: ComponentFactoryResolver,
    public dialogRef: MatDialogRef<FullscreenModalComponent>,
    @Inject(MAT_DIALOG_DATA) public data: FullscreenModalComponentInput) { 
  }

  ngOnInit() {
    this.loadContentComponent(this.data);
  }

  ngOnDestroy() {
    this.destroy();
  }

  loadContentComponent(inputData: FullscreenModalComponentInput) {
    let componentFactory = this.componentFactoryResolver.resolveComponentFactory(inputData.component);
    let viewContainerRef = this.modalHost.viewContainerRef;
    viewContainerRef.clear();
    this.destroy();
    if (inputData.hiddenClose) {
      this.dialogRef.disableClose = true;
    } else {
      this.dialogRef.disableClose = false;
    }
    let componentRef = viewContainerRef.createComponent(componentFactory);
    this.modalComponent = (<ModalComponent>componentRef.instance);
    this.title = this.modalComponent.modalTitle;
    this.modalComponent.data = inputData.componentInput;
    this.modalComponent.setClose.subscribe(() => this.dialogRef.close());
    this.modalComponent.setNewModal.subscribe(newModalInput => 
      {
        this.data = newModalInput;
        this.loadContentComponent(newModalInput);
      });
  }

  destroy() {
    if (!!this.modalComponent) {
      this.modalComponent.setClose.unsubscribe();
      this.modalComponent.setNewModal.unsubscribe();
    }
  }
}
