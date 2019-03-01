import { Routes, RouterModule } from '@angular/router';

import { ProfileComponent } from './profile';
import { LoginComponent } from './login';
import { LogoutComponent } from './logout';
import { RegisterComponent } from './register';
import { InterviewTemplateComponent } from './interviewtemplate';
import { InterviewComponent } from './interview';
import { ReviewComponent } from './review';
import { QuestionComponent } from './question';
import { UnsubscribeComponent } from './unsubscribe';
import { AuthGuard } from './_guards';

const appRoutes: Routes = [
    { path: 'profile', component: ProfileComponent },
    { path: 'login', component: LoginComponent },
    { path: 'logout', component: LogoutComponent },
    { path: 'register', component: RegisterComponent },
    { path: 'interviewtemplate', component: InterviewTemplateComponent, canActivate: [AuthGuard] },
    { path: 'interview', component: InterviewComponent },
    { path: 'review', component: ReviewComponent },
    { path: 'question', component: QuestionComponent },
    { path: 'unsubscribe', component: UnsubscribeComponent },

    // otherwise redirect to home
    { path: '**', redirectTo: 'interviewtemplate' }
];

export const routing = RouterModule.forRoot(appRoutes);
