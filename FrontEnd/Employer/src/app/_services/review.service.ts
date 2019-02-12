import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { environment } from '@environments/environment';
import { Video } from '@app/_models';

@Injectable({ providedIn: 'root' })
export class ReviewService {
  constructor(private http: HttpClient) { }

  getAll() {
    return this.http.get<Video[]>(`${environment.apiUrl}/video`);
  }

  getByInterviewId(interviewId: string) {
    return this.http.get<Video[]>(`${environment.apiUrl}/video/${interviewId}`);
  }
}
