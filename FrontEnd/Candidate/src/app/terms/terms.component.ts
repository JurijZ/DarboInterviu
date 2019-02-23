import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';

import { AlertService, AuthenticationService } from '@app/_services';

@Component({
    templateUrl: './terms.component.html',
    styleUrls: ['./terms.component.css']
})
export class TermsComponent implements OnInit {
    language = 'LT';

    constructor(
        private route: ActivatedRoute,
        private router: Router,
        private authenticationService: AuthenticationService,
        private alertService: AlertService
    ) {
    }

    ngOnInit() {
    }

    setLanguageLT(){
        this.language = 'LT';
    }

    setLanguageEN(){
        this.language = 'EN';
    }
}



