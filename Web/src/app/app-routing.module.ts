import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TopAdvisorsComponent } from './components/advisor/top-advisors/top-advisors.component';

const routes: Routes = [
    { path: '', redirectTo: 'home', pathMatch: 'full' },
    { path: 'top-advisors', component: TopAdvisorsComponent },
];

@NgModule({
    exports: [RouterModule],
    imports: [RouterModule.forRoot(routes)]
})

export class AppRoutingModule { }
