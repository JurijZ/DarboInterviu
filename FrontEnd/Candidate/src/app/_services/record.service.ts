import { Injectable } from '@angular/core';
import { HttpClient, HttpRequest, HttpEventType, HttpResponse } from '@angular/common/http';

import { environment } from '@environments/environment';
import { Question, Application } from '@app/_models';
import { stringify } from 'querystring';

@Injectable({ providedIn: 'root' })
export class RecordService {

  constructor(private http: HttpClient) {

  }

  getQuestionsByApplicationId(id: string) {
    console.log("Requesting quetions for the applicationId: " + id);
    return this.http.get<Question[]>(`${environment.apiUrl}/Candidate/questions/${id}`);
  }

  updateInterviewStatus(id: string, status: string) {
    console.log("Current interview id: " + id);
    var interviewStatus = { applicationId: id, status: status };

    return this.http.post(`${environment.apiUrl}/Candidate/status`, interviewStatus);
  }

  sendToServer(applicationId: string, questionId: string, blob: Blob) {
    const formData = new FormData();

    var filename = questionId + ".webm";
    console.log(filename);

    formData.append('video-blob', blob, filename);
    console.log("Record blob size: " + blob.size);

    formData.append('applicationId', applicationId);
    formData.append('questionId', questionId);

    let baseUrl = environment.apiUrl + '/upload'; // WebAPI project
    console.log("BaseURL is: " + baseUrl);

    const uploadReq = new HttpRequest('POST', baseUrl, formData, {
      reportProgress: true,
    });

    return this.http.request(uploadReq);
  }

  //download() {
  //   this.recordRTC.save('video.webm');
  // }

  private currentTimeStamp() {
    return new Date().toISOString().replace(/:\s*/g, "");
  }

  upload(files) {
    if (files.length === 0)
      return;

    const formData = new FormData();

    for (let file of files)
      formData.append(file.name, file);

    let baseUrl = environment.apiUrl + '/upload'; // WebAPI project
    console.log("BaseURL is: " + baseUrl);

    const uploadReq = new HttpRequest('POST', baseUrl, formData, {
      reportProgress: true,
    });

    return this.http.request(uploadReq);
  }
}