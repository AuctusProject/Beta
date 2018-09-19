import { NgModule } from '@angular/core';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome'
import { library } from '@fortawesome/fontawesome-svg-core';
import { faLock, faChevronUp, faChevronDown, faTimes, faCheck, faArrowRight } from '@fortawesome/free-solid-svg-icons';
import { faTwitter, faFacebookF, faTelegramPlane, faGoogle } from '@fortawesome/free-brands-svg-icons';
import { IconEye, IconEyeOff, IconSearch } from 'angular-feather';

library.add(faLock, faChevronUp, faChevronDown, faTwitter, faFacebookF, faTelegramPlane, faGoogle, faTimes, faCheck, faArrowRight);
@NgModule({
   imports: [FontAwesomeModule, IconEye, IconEyeOff, IconSearch],
   exports: [FontAwesomeModule, IconEye, IconEyeOff, IconSearch]
})
export class IconsModule { }
