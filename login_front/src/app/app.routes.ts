import { Routes } from '@angular/router';
import { MainComponent } from './components/main/main.component';
import { FunctionalityPageComponent } from './components/functionality-page/functionality-page.component';

export const routes: Routes = [
    {path:'',component:MainComponent},
    {path:'functionalityPage',component:FunctionalityPageComponent}
];
