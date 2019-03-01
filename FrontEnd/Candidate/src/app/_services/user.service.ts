import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { environment } from '@environments/environment';
import { User, Application } from '@app/_models';

@Injectable({ providedIn: 'root' })
export class UserService {
    constructor(private http: HttpClient) { }

    unsubscribeEmail(email: string){
        return this.http.put(`${environment.apiUrl}/candidate/unsubscribe/${email}`, null);
    }
}