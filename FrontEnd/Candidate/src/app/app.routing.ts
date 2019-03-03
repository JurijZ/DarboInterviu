import { Routes, RouterModule } from '@angular/router';

import { HomeComponent } from './home';
import { LoginComponent } from './login';
import { LogoutComponent } from './logout';
import { TermsComponent } from './terms';
import { RecordRTCComponent } from './record-rtc';
import { TestComponent } from './test';
import { FinalComponent } from './final';
import { AuthGuard } from './_guards';
import { UnsubscribeComponent } from './unsubscribe';

const appRoutes: Routes = [
    { path: 'home', component: HomeComponent, canActivate: [AuthGuard] },
    { path: 'login', component: LoginComponent },
    { path: 'logout', component: LogoutComponent },
    { path: 'terms', component: TermsComponent },
    { path: 'record-rtc', component: RecordRTCComponent },
    { path: 'test', component: TestComponent },
    { path: 'final', component: FinalComponent },
    { path: 'unsubscribe', component: UnsubscribeComponent },

    // otherwise redirect to home
    { path: '**', redirectTo: 'home' }
];

export const routing = RouterModule.forRoot(appRoutes);
