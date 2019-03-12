import { NgModule }      from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { ReactiveFormsModule, FormsModule  }    from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';

// used to create fake backend
//import { fakeBackendProvider } from './_interceptors';

import { AppComponent }  from './app.component';
import { routing }        from './app.routing';

import { JwtInterceptor, ErrorInterceptor } from './_interceptors';

import { AlertComponent } from './_components/alert';
import { ProfileComponent } from './_components/profile';
import { LoginComponent } from './_components/login';
import { LogoutComponent } from './_components/logout';
import { RegisterComponent } from './_components/register';
import { InterviewTemplateComponent } from './_components/interviewtemplate';
import { InterviewModalComponent } from './_components/interviewmodal';
import { InterviewComponent } from './_components/interview';
import { InterviewShareComponent } from './_components/interviewshare';
import { ReviewComponent } from './_components/review';
import { QuestionComponent } from './_components/question';
import { UnsubscribeComponent } from './_components/unsubscribe';

import { TextAreaRowsDirective } from './_directives';

@NgModule({
    imports: [
        BrowserModule,
        ReactiveFormsModule,
        FormsModule,
        HttpClientModule,
        routing,
        NgbModule
    ],
    declarations: [
        AppComponent,
        AlertComponent,
        ProfileComponent,
        LoginComponent,
        LogoutComponent,
        RegisterComponent,
        InterviewTemplateComponent,
        InterviewModalComponent,
        InterviewComponent,
        InterviewShareComponent,
        ReviewComponent,
        QuestionComponent,
        UnsubscribeComponent,
        TextAreaRowsDirective
    ],
    providers: [
        { provide: HTTP_INTERCEPTORS, useClass: JwtInterceptor, multi: true },
        { provide: HTTP_INTERCEPTORS, useClass: ErrorInterceptor, multi: true },

        // provider used to create fake backend
        //fakeBackendProvider
    ],
    bootstrap: [AppComponent]
})

export class AppModule { }
