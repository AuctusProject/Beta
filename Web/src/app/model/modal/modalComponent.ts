import { EventEmitter } from "@angular/core";

export interface ModalComponent {
    setClose: EventEmitter<void>;
    data: any;
}