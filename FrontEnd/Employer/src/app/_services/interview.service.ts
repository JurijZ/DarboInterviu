import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { environment } from '@environments/environment';
import { Interview, Share } from '@app/_models';

@Injectable({ providedIn: 'root' })
export class InterviewService {
  constructor(private http: HttpClient) { }

  getAllByUserId(id: string) {
    return this.http.get<Interview[]>(`${environment.apiUrl}/application/${id}`);
  }

  shareInterview(share: Share){
    return this.http.post<string>(`${environment.apiUrl}/application/share`, share);
  }
}
