import { EventEmitter } from "@angular/core";
import { FullscreenModalComponentInput } from "./fullscreenModalComponentInput";

export interface ModalComponent {
    setClose: EventEmitter<void>;
    setNewModal: EventEmitter<FullscreenModalComponentInput>;
    data: any;
}