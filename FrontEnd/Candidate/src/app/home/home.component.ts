import { Component, OnInit, OnDestroy } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { Subscription } from 'rxjs';
import { first } from 'rxjs/operators';

import { User, Application } from '@app/_models';
import { HomeService, AuthenticationService } from '@app/_services';

@Component({ templateUrl: 'home.component.html' })
export class HomeComponent implements OnInit, OnDestroy {
    currentUser: User;
    currentUserSubscription: Subscription;
    users: User[] = [];
    application: Application = new Application();

    constructor(private authenticationService: AuthenticationService,
        private homeService: HomeService,
        private router: Router) {
        this.currentUserSubscription = this.authenticationService.currentUser.subscribe(user => {
            this.currentUser = user;

            this.loadApplication(this.currentUser.applicationId);
        });
    }

    ngOnInit() {
    }

    ngOnDestroy() {
        // unsubscribe to ensure no memory leaks
        this.currentUserSubscription.unsubscribe();
    }

    private loadApplication(applicationid: string) {
        this.homeService.getApplicationById(applicationid).pipe(first()).subscribe(application => {
            this.application = application;
            console.log(this.application);
        });
    }

    startInterview() {
        //console.log(this.application);
        this.router.navigate(['/record-rtc']);
    }

    testRecording() {
        this.router.navigate(['/test']);
    }
}