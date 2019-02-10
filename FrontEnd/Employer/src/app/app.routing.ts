import { Routes, RouterModule } from '@angular/router';

import { HomeComponent } from './home';
import { LoginComponent } from './login';
import { RegisterComponent } from './register';
import { VideosComponent } from './videos';
import { InterviewTemplateComponent } from './interviewtemplate';
import { InterviewComponent } from './interview';
import { ReviewComponent } from './review';
import { QuestionComponent } from './question';
import { RecordRTCComponent } from './record-rtc';
import { AuthGuard } from './_guards';

const appRoutes: Routes = [
    { path: '', component: HomeComponent, canActivate: [AuthGuard] },
    { path: 'login', component: LoginComponent },
    { path: 'register', component: RegisterComponent },
    { path: 'videos', component: VideosComponent },
    { path: 'interviewtemplate', component: InterviewTemplateComponent },
    { path: 'interview', component: InterviewComponent },
    { path: 'review', component: ReviewComponent },
    { path: 'question', component: QuestionComponent },
    { path: 'record-rtc', component: RecordRTCComponent },

    // otherwise redirect to home
    { path: '**', redirectTo: '' }
];

export const routing = RouterModule.forRoot(appRoutes);
