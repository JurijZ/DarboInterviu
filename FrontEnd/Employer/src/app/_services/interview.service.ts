import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { environment } from '@environments/environment';
import { Interview } from '@app/_models';

@Injectable({ providedIn: 'root' })
export class InterviewService {
    constructor(private http: HttpClient) { }

    getAll() {
      return this.http.get<Interview[]>(`${environment.apiUrl}/interview`);
    }

    getById(id: string) {
        return this.http.get(`${environment.apiUrl}/interview/${id}`);
    }

  creaete(interview: Interview) {
    return this.http.post(`${environment.apiUrl}/interview/create`, interview);
    }

  update(interview: Interview) {
    return this.http.put(`${environment.apiUrl}/interview/${interview.id}`, interview);
    }

    delete(id: string) {
        return this.http.delete(`${environment.apiUrl}/interview/${id}`);
    }
}
