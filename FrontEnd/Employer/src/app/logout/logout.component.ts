import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';

import { AuthenticationService } from '@app/_services';

@Component({templateUrl: 'logout.component.html'})
export class LogoutComponent implements OnInit {

    constructor(
        private router: Router,
        private authenticationService: AuthenticationService
    ) {
        this.authenticationService.logout();
    }

    ngOnInit() {        
        this.router.navigate(['/login']);        
    }
}
