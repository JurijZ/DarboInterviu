import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { environment } from '@environments/environment';
import { Question, Application } from '@app/_models';
import { stringify } from 'querystring';

@Injectable({ providedIn: 'root' })
export class RecordService {
    constructor(private http: HttpClient) { } 

    getQuestionsByApplicationId(id: string) {
        console.log("Requesting quetions for the applicationId: " + id);
        return this.http.get<Question[]>(`${environment.apiUrl}/Candidate/questions/${id}`);
    }

    updateInterviewStatus(id: string, status: string) {
        console.log("Current interview id: " + id);
        var interviewStatus = {applicationId: id, status: status};

        return this.http.post(`${environment.apiUrl}/Candidate/status`, interviewStatus);
    }
}