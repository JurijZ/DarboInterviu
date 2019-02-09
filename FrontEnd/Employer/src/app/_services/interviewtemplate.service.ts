import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { environment } from '@environments/environment';
import { InterviewTemplate, Application } from '@app/_models';

@Injectable({ providedIn: 'root' })
export class InterviewTemplateService {
  constructor(private http: HttpClient) { }

  getAll() {
    return this.http.get<InterviewTemplate[]>(`${environment.apiUrl}/interview`);
  }

  getById(id: string) {
    return this.http.get(`${environment.apiUrl}/interview/${id}`);
  }

  create(interview: InterviewTemplate) {
    return this.http.post<InterviewTemplate>(`${environment.apiUrl}/interview/create`, interview);
  }

  createApplication(application: Application) {
    return this.http.post(`${environment.apiUrl}/application/create`, application);
  }

  update(interview: InterviewTemplate) {
    return this.http.put(`${environment.apiUrl}/interview/${interview.id}`, interview);
  }

  delete(id: string) {
    return this.http.delete(`${environment.apiUrl}/interview/${id}`);
  }
}
