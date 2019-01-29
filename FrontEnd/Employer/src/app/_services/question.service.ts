import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { environment } from '@environments/environment';
import { Interview, Question } from '@app/_models';

@Injectable({ providedIn: 'root' })
export class QuestionService {
  constructor(private http: HttpClient) { }

  getAll() {
    return this.http.get<Question[]>(`${environment.apiUrl}/question`);
  }

  getById(id: string) {
    return this.http.get(`${environment.apiUrl}/question/${id}`);
  }

  creaete(question: Question) {
    return this.http.post(`${environment.apiUrl}/question/create`, question);
  }

  update(question: Question) {
    return this.http.put(`${environment.apiUrl}/question/${question.id}`, question);
  }

  delete(id: string) {
    return this.http.delete(`${environment.apiUrl}/question/${id}`);
  }
}
