import { Routes, RouterModule } from '@angular/router';

import { HomeComponent } from './home';
import { LoginComponent } from './login';
import { LogoutComponent } from './logout';
import { RegisterComponent } from './register';
import { VideosComponent } from './videos';
import { InterviewTemplateComponent } from './interviewtemplate';
import { InterviewComponent } from './interview';
import { ReviewComponent } from './review';
import { QuestionComponent } from './question';
import { AuthGuard } from './_guards';

const appRoutes: Routes = [
    { path: 'home', component: HomeComponent, canActivate: [AuthGuard] },
    { path: 'login', component: LoginComponent },
    { path: 'logout', component: LogoutComponent },
    { path: 'register', component: RegisterComponent },
    { path: 'videos', component: VideosComponent },
    { path: 'interviewtemplate', component: InterviewTemplateComponent },
    { path: 'interview', component: InterviewComponent },
    { path: 'review', component: ReviewComponent },
    { path: 'question', component: QuestionComponent },

    // otherwise redirect to home
    { path: '**', redirectTo: 'home' }
];

export const routing = RouterModule.forRoot(appRoutes);
