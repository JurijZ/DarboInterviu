import { Routes, RouterModule } from '@angular/router';

import { ProfileComponent } from './_components/profile';
import { LoginComponent } from './_components/login';
import { LogoutComponent } from './_components/logout';
import { RegisterComponent } from './_components/register';
import { InterviewTemplateComponent } from './_components/interviewtemplate';
import { InterviewComponent } from './_components/interview';
import { ReviewComponent } from './_components/review';
import { QuestionComponent } from './_components/question';
import { UnsubscribeComponent } from './_components/unsubscribe';
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
