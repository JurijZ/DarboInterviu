import { Component, Inject, OnInit, ViewChild } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '@environments/environment';
import { Subscription } from 'rxjs';
import { first } from 'rxjs/operators';

import { DataService, ReviewService, AuthenticationService } from '@app/_services';
import { Interview , Video } from '@app/_models';

@Component({
  selector: 'review',
  templateUrl: './review.component.html',
  styleUrls: ['./review.component.css']
})
export class ReviewComponent implements OnInit {
  public videos: Video[];
  public videoName: string; 
  public activeQuestion: string = "";
  public url: string;
  public urlBackgroundImage: string = environment.apiUrl + "/Upload/Poster.png";
  public baseUrl: string;
  public interview: Interview;
  public selectedButton: number = -1;
  @ViewChild('myVideo') myVideo: any;
  
  constructor(
      http: HttpClient,
      private data: DataService,
      private reviewService: ReviewService
    ) {
  }

  ngOnInit() {
    this.data.currentInterview.subscribe(interview => {
      this.interview = interview;
      console.log(this.interview);

      this.reviewService.getByInterviewId(this.interview.id).pipe(first()).subscribe(videos => {
        this.videos = videos;

        this.selectedButton = this.videos[0].videoId;
        this.showQuestion(this.videos[0].videoId);
      });
    })
  }

  playVideo(fileName: string) {
    console.log("Selected video: " + fileName);

    /* You are accessing a dom element directly here, so you need to call "nativeElement" first. */
    this.myVideo.nativeElement.src = environment.apiUrl + "/Video/record/" + fileName;
    this.myVideo.nativeElement.play();
    this.videoName = fileName;
  } 

  showQuestion(videoId: number){
    var video = this.videos.find(v => v.videoId === videoId);
    console.log(video.question);

    this.activeQuestion = video.question;

    this.playVideo(video.videoFileName)
  }
}

export interface IMedia {
  title: string;
  src: string;
  type: string;
}
