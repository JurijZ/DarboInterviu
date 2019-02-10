import { Component, Inject, OnInit, ViewChild } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '@environments/environment';

import { DataService, QuestionService, AuthenticationService } from '@app/_services';
import { Interview } from '@app/_models';

@Component({
  selector: 'review',
  templateUrl: './review.component.html',
  styleUrls: ['./review.component.css']
})
export class ReviewComponent implements OnInit {
  public videos: Video[];
  public videoName: string; 
  public url: string;
  public baseUrl: string;
  public interview: Interview;
  @ViewChild('myVideo') myVideo: any;
  
  constructor(
      http: HttpClient,
      private data: DataService
    ) {
    this.baseUrl = environment.apiUrl + '/Video'; // WebAPI project - visi video 
    console.log("BaseURL is: " + this.baseUrl);
    //this.baseUrl = document.getElementsByTagName('base')[0].href; //Angular project
    
    http.get<Video[]>(this.baseUrl).subscribe(result => {
      this.videos = result;
    }, error => console.error(error));
  }

  ngOnInit() {
    this.data.currentInterview.subscribe(interview => {
      this.interview = interview;
      console.log(this.interview);
      //this.loadAllQuestions(this.interviewTemplate.id);
    })
  }

  playVideo(fileName: string) {
    console.log("Selected video: " + fileName);

    /* You are accessing a dom element directly here, so you need to call "nativeElement" first. */
    this.myVideo.nativeElement.src = environment.apiUrl + "/Video/" + fileName;
    this.myVideo.nativeElement.play();
    this.videoName = fileName;
  } 
}

interface Video {
  name: string;
  candidate: string;
  timestamp: string;
}

export interface IMedia {
  title: string;
  src: string;
  type: string;
}
