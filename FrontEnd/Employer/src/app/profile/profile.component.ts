import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs';
import { first } from 'rxjs/operators';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

import { User } from '@app/_models';
import { UserService, AuthenticationService } from '@app/_services';

@Component({ templateUrl: 'profile.component.html' })
export class ProfileComponent implements OnInit, OnDestroy {
    currentUser: User;
    currentUserSubscription: Subscription;

    changeUserPropertiesForm: FormGroup;

    changePasswordForm: FormGroup;
    passwordFormVisible: boolean = false;
    submitted: boolean = false;
    passwordIsChanged: string = '';
    newPassword: string = '';

    constructor(
        private formBuilder: FormBuilder,
        private authenticationService: AuthenticationService,
        private userService: UserService
    ) {
        this.currentUserSubscription = this.authenticationService.currentUser.subscribe(user => {
            this.currentUser = user;
        });
    }

    ngOnInit() {
        this.changePasswordForm = this.formBuilder.group({
            password: ['', [Validators.required, Validators.minLength(6)]]
        });

        this.changeUserPropertiesForm = this.formBuilder.group({
            firstname: ['', Validators.required],
            lastName: ['', Validators.required]
        });
    }

    ngOnDestroy() {
        // unsubscribe to ensure no memory leaks
        this.currentUserSubscription.unsubscribe();
    }

    toggleChangePasswordForm() {
        this.passwordFormVisible = !this.passwordFormVisible;
    }

    // convenience getter for easy access to form fields (used in html for validation)
    get f() { return this.changePasswordForm.controls; }

    changePassword() {
        this.submitted = true;
        this.passwordIsChanged = "";

        // stop here if form is invalid
        if (this.changePasswordForm.invalid) {
            return;
        }

        this.currentUser.password = this.changePasswordForm.controls['password'].value;
        console.log(this.currentUser);

        this.userService.changePassword(this.currentUser)
            .pipe(first())
            .subscribe(
                data => {
                    this.passwordIsChanged = "Naujas slaptažodis buvo sėkmingai išsaugotas, Ačiū.";
                    this.toggleChangePasswordForm()
                    console.log(this.passwordIsChanged);
                },
                error => {
                    this.passwordIsChanged = "Įvyko klaida.";
                    console.log(this.passwordIsChanged);
                });
    }
}
