import { Routes, RouterModule } from '@angular/router';

import { HomeComponent } from './home';
import { LoginComponent } from './login';
import { RegisterComponent } from './register';
import { VideosComponent } from './videos';
import { InterviewComponent } from './interview';
import { QuestionComponent } from './question';
import { RecordRTCComponent } from './record-rtc';
import { AuthGuard } from './_guards';

const appRoutes: Routes = [
    { path: '', component: HomeComponent, canActivate: [AuthGuard] },
    { path: 'login', component: LoginComponent },
    { path: 'register', component: RegisterComponent },
  { path: 'videos', component: VideosComponent },
  { path: 'interview', component: InterviewComponent },
  { path: 'question', component: QuestionComponent },
    { path: 'record-rtc', component: RecordRTCComponent },

    // otherwise redirect to home
    { path: '**', redirectTo: '' }
];

export const routing = RouterModule.forRoot(appRoutes);
