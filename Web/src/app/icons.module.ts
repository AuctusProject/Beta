import { NgModule } from '@angular/core';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome'
import { library } from '@fortawesome/fontawesome-svg-core';
import { faLock, faChevronUp, faChevronDown, faChevronRight, faTimes, faCheck, faArrowRight, faCircle } from '@fortawesome/free-solid-svg-icons';
import { faTwitter, faFacebookF, faTelegramPlane, faGoogle } from '@fortawesome/free-brands-svg-icons';
import { IconEye, IconEyeOff, IconSearch } from 'angular-feather';

library.add(faLock, faChevronUp, faChevronDown, faChevronRight, faTwitter, faFacebookF, faTelegramPlane, faGoogle, faTimes, faCheck, faArrowRight, faCircle);
@NgModule({
   imports: [FontAwesomeModule, IconEye, IconEyeOff, IconSearch],
   exports: [FontAwesomeModule, IconEye, IconEyeOff, IconSearch]
})
export class IconsModule { }
