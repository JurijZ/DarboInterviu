import { NgModule }      from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { ReactiveFormsModule, FormsModule  }    from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';

// used to create fake backend
//import { fakeBackendProvider } from './_helpers';

import { AppComponent }  from './app.component';
import { routing }        from './app.routing';

import { JwtInterceptor, ErrorInterceptor } from './_helpers';

import { AlertComponent } from './alert';
import { ProfileComponent } from './profile';
import { LoginComponent } from './login';
import { LogoutComponent } from './logout';
import { RegisterComponent } from './register';
import { InterviewTemplateComponent } from './interviewtemplate';
import { InterviewModalComponent } from './interviewmodal';
import { InterviewComponent } from './interview';
import { InterviewShareComponent } from './interviewshare';
import { ReviewComponent } from './review';
import { QuestionComponent } from './question';
import { UnsubscribeComponent } from './unsubscribe';

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
