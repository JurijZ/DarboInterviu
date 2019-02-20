import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { environment } from '@environments/environment';
import { Video } from '@app/_models';

@Injectable({ providedIn: 'root' })
export class VideoService {
  constructor(private http: HttpClient) { }

  getAllVideos() {
    return this.http.get<Video[]>(`${environment.apiUrl}/support/video`);
  } 
}
