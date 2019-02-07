import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { environment } from '@environments/environment';
import { Application } from '@app/_models';

@Injectable({ providedIn: 'root' })
export class HomeService {
    constructor(private http: HttpClient) { } 

    getApplicationById(id: string) {
        return this.http.get<Application>(`${environment.apiUrl}/candidate/${id}`);
    }

}