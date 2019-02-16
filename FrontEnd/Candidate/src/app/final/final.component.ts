import { Component, OnInit, OnDestroy } from '@angular/core';
import { Router } from '@angular/router';
import { Subscription } from 'rxjs';

import { User } from '@app/_models';
import { AuthenticationService } from '@app/_services';

@Component({ templateUrl: 'final.component.html' })
export class FinalComponent implements OnInit, OnDestroy {
    currentUser: User;
    currentUserSubscription: Subscription;

    constructor(
        private authenticationService: AuthenticationService, 
        private router: Router) {
    }

    ngOnInit() {
        this.currentUserSubscription = this.authenticationService.currentUser.subscribe(user => {
            this.currentUser = user;
            //console.log(this.currentUser);
        });
    }

    ngOnDestroy() {
        // unsubscribe to ensure no memory leaks
        this.currentUserSubscription.unsubscribe();
    }

    logOut() {
        this.authenticationService.logout();
        this.router.navigate(['/login']);        
    }
}