import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { environment } from '@environments/environment';
import { InterviewTemplate, Question } from '@app/_models';

@Injectable({ providedIn: 'root' })
export class QuestionService {
  constructor(private http: HttpClient) { }

  getAll() {
    return this.http.get<Question[]>(`${environment.apiUrl}/question`);
  }

  getByTemplateId(templateId: string) {
    return this.http.get<Question[]>(`${environment.apiUrl}/question/template/${templateId}`);
  }

  getById(id: string) {
    return this.http.get(`${environment.apiUrl}/question/${id}`);
  }

  create(question: Question) {
    return this.http.post(`${environment.apiUrl}/question/create`, question);
  }

  update(question: Question) {
    return this.http.put(`${environment.apiUrl}/question/${question.id}`, question);
  }

  updateInterview(interview: InterviewTemplate) {
    return this.http.put(`${environment.apiUrl}/template/${interview.id}`, interview);
  }

  delete(id: string) {
    return this.http.delete(`${environment.apiUrl}/question/${id}`);
  }
}
