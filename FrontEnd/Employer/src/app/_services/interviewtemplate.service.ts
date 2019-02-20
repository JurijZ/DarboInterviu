import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { environment } from '@environments/environment';
import { InterviewTemplate, Application } from '@app/_models';

@Injectable({ providedIn: 'root' })
export class InterviewTemplateService {
  constructor(private http: HttpClient) { }

  /*
  getAll() {
    return this.http.get<InterviewTemplate[]>(`${environment.apiUrl}/template`);
  }
  */

  getAllByUserId(userId: string) {
    return this.http.get<InterviewTemplate[]>(`${environment.apiUrl}/template/${userId}`);
  }

  create(interview: InterviewTemplate) {
    return this.http.post<InterviewTemplate>(`${environment.apiUrl}/template/create`, interview);
  }

  createApplication(application: Application) {
    return this.http.post(`${environment.apiUrl}/application/create`, application);
  }

  update(interview: InterviewTemplate) {
    return this.http.put(`${environment.apiUrl}/template/${interview.id}`, interview);
  }

  delete(id: string) {
    return this.http.delete(`${environment.apiUrl}/template/${id}`);
  }
}
