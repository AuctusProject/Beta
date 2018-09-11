import { NgModule } from '@angular/core';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome'
import { library } from '@fortawesome/fontawesome-svg-core';
import { faLock  } from '@fortawesome/free-solid-svg-icons';
import { faTwitter, faFacebookF, faLinkedinIn } from '@fortawesome/free-brands-svg-icons';
import { IconEye, IconEyeOff, IconSearch } from 'angular-feather';

library.add(faLock, faTwitter, faFacebookF, faLinkedinIn);
@NgModule({
   imports: [FontAwesomeModule, IconEye, IconEyeOff, IconSearch],
   exports: [FontAwesomeModule, IconEye, IconEyeOff, IconSearch]
})
export class IconsModule { }
