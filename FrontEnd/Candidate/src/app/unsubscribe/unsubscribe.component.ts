import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { first } from 'rxjs/operators';

import { UserService } from '@app/_services';

@Component({templateUrl: 'unsubscribe.component.html'})
export class UnsubscribeComponent implements OnInit {
    email: string;
    unsubscribeStatus: string = '';
    successMessage: string = "Jūs sėkmingai atsisakėte informacinių pranešimų, Ačiū.";
    failureMessage: string = "Deja įvyko klaida, pabandykite dar kartą.";
    submitted: boolean = false;

    constructor(
        private route: ActivatedRoute,
        private userService: UserService
    ) {
        this.route.queryParams.subscribe(params => {
            this.email = params['email'];
            console.log(this.email); // Print the URI parameter to the console. 
        });
    }

    ngOnInit() {
    }

    unsubscribe(){
        console.log("Unsubscribing email address: " + this.email);

        this.submitted = true;
        this.unsubscribeStatus = "";

        this.userService.unsubscribeEmail(this.email)
            .pipe(first())
            .subscribe(
                data => {
                    this.unsubscribeStatus = "Succeded";
                    console.log(this.unsubscribeStatus);
                },
                error => {
                    this.unsubscribeStatus = "Failed";
                    console.log(this.unsubscribeStatus);
                });
    }
}
