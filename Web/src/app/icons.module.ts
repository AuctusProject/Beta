import { NgModule } from '@angular/core';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome'
import { library } from '@fortawesome/fontawesome-svg-core';
import { faStar as faStarRegular, faListAlt } from '@fortawesome/free-regular-svg-icons';
import { faQuestionCircle, faAward, faShareAlt, faEdit, faSignOutAlt, faSignInAlt, faStar, faChartLine, faWallet, faList, faLock, faBook, faLayerGroup, faChevronUp, faChevronDown, faChevronRight, faTimes, faCheck, faArrowRight, faCircle, faDownload } from '@fortawesome/free-solid-svg-icons';
import { faTwitter, faFacebookF, faTelegramPlane, faGoogle } from '@fortawesome/free-brands-svg-icons';
import { IconEye, IconEyeOff, IconSearch } from 'angular-feather';

library.add(faAward, faShareAlt, faStarRegular, faEdit, faSignOutAlt, faSignInAlt, faStar, faChartLine, faWallet, faList, faLock, faBook, faChevronUp, faLayerGroup, faChevronDown, faChevronRight, faTwitter, faFacebookF, faTelegramPlane, faGoogle, faTimes, faCheck, faArrowRight, faCircle, faDownload, faListAlt, faQuestionCircle);
@NgModule({
   imports: [FontAwesomeModule, IconEye, IconEyeOff, IconSearch],
   exports: [FontAwesomeModule, IconEye, IconEyeOff, IconSearch]
})
export class IconsModule { }
