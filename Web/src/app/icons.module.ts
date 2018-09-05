import { NgModule } from '@angular/core';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome'
import { library } from '@fortawesome/fontawesome-svg-core';
import { faLock } from '@fortawesome/free-solid-svg-icons';

library.add(faLock);
@NgModule({
   imports: [FontAwesomeModule],
   exports: [FontAwesomeModule]
})
export class IconsModule { }
