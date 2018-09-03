import { Component, OnInit, ElementRef, ViewChild } from "@angular/core";
import { NotificationsService } from "angular2-notifications";

@Component({
    selector: 'file-uploader',
    templateUrl: './file-uploader.component.html',
    styleUrls: ['./file-uploader.component.css'],
    inputs:['pictureUrl']
})
export class FileUploaderComponent {
    
    activeColor: string = '#17c723';
    baseColor: string = '#161616';
    activeBox: string = '0 0 9px #17c723';
    baseBox: string = '0 0 4px #161616';
    overlayColor: string = 'rgba(15, 15, 15, 0.5)';
    pictureUrl: string = '';
    
    dragging: boolean = false;
    loaded: boolean = false;
    imageLoaded: boolean = false;
    imageSrc: string = '';
    iconColor: string = '';
    fileToUpload: File = null;
    imageText: string = 'Click here to change';
    wasChanged: boolean = false;
    @ViewChild("fileInput") fileInput: ElementRef;
    
    constructor(private notificationService: NotificationsService) { }

    ngOnInit() {
        this.imageSrc = this.pictureUrl;
        if (this.imageSrc) {
            this.imageLoaded = true;
        }
    }

    handleDragEnter() {
        this.dragging = true;
    }
    
    handleDragLeave() {
        this.dragging = false;
    }
    
    handleDrop(e) {
        e.preventDefault();
        this.dragging = false;
        this.handleInputChange(e);
    }
    
    handleImageLoad() {
        this.imageLoaded = true;
        this.iconColor = this.overlayColor;
    }

    handleInputChange(e) {
        let file = e.dataTransfer ? e.dataTransfer.files[0] : e.target.files[0];
        if (file) {
            this.loaded = false;
            this.fileToUpload = file;

            let type = file.type.toLowerCase();
            if (type.indexOf("png") >= 0 || type.indexOf("jpeg") >= 0 || type.indexOf("jpg") >= 0) {
                this.wasChanged = true;
                this.imageText = '';
                let reader = new FileReader();
                reader.onload = this._handleReaderLoaded.bind(this);
                reader.readAsDataURL(file);
            } else {
                this.notificationService.error(
                    "Error",
                    "Invalid file type."
                  );
                this.clearComponent();
            }
        } else {
            this.clearComponent();
        }
    }

    getFile() : File {
        return this.fileToUpload;
    }

    fileWasChanged() : boolean {
        return this.wasChanged;
    }

    clearComponent() {
        this.imageSrc = '';
        this.imageLoaded = false;
        this.imageText = 'Click here to change';
        this.loaded = false;
        this.fileToUpload = null;
        this.fileInput.nativeElement.value = '';
    }
    
    _handleReaderLoaded(e) {
        var reader = e.target;
        this.imageSrc = reader.result;
        this.loaded = true;
        this.imageText = '';
    }

    removeImage(e) {
        e.preventDefault();
        e.stopPropagation();
        this.clearComponent();
    }
}