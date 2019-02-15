import { Injectable } from '@angular/core';
import { HttpClient, HttpRequest, HttpEventType, HttpResponse } from '@angular/common/http';

import { environment } from '@environments/environment';
import { Question, Application } from '@app/_models';
import { stringify } from 'querystring';

@Injectable({ providedIn: 'root' })
export class TestService {
    constructor(private http: HttpClient) { } 

    sendToServer(testId: string, blob: Blob) {
        const formData = new FormData();
    
        var filename = testId + ".webm";
        console.log(filename);
    
        formData.append('video-blob', blob, filename);
        console.log("Record blob size: " + blob.size);

        formData.append('applicationId', testId);
    
        let baseUrl = environment.apiUrl + '/test'; // WebAPI project
        console.log("BaseURL is: " + baseUrl);
    
        const uploadReq = new HttpRequest('POST', baseUrl, formData, {
          reportProgress: true,
        });
    
        return this.http.request(uploadReq);
    }
}