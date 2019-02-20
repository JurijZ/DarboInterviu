import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { environment } from '@environments/environment';
import { Interview, Application } from '@app/_models';

@Injectable({ providedIn: 'root' })
export class InterviewService {
  constructor(private http: HttpClient) { }

  getAll() {
    return this.http.get<Interview[]>(`${environment.apiUrl}/support/application`);
  }
}
