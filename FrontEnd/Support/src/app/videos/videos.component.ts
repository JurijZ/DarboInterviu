import { Component, Inject, OnInit, ViewChild } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '@environments/environment';
import { first } from 'rxjs/operators';

import { DataService, VideoService, AuthenticationService } from '@app/_services';
import { Interview, Video } from '@app/_models';

@Component({
  selector: 'videos',
  templateUrl: './videos.component.html',
  styleUrls: ['./videos.component.css']
})
export class VideosComponent implements OnInit {
  public videos: Video[];
  public videoName: string;
  @ViewChild('myVideo') myVideo: any;
  public url: string;
  public baseUrl: string;

  constructor(
    http: HttpClient,
    private data: DataService,
    private videoService: VideoService
  ) {
  }

  ngOnInit() {
    this.videoService.getAllVideos().pipe(first()).subscribe(videos => {
      this.videos = videos;
    });
  }

  playVideo(fileName: string) {
    console.log("Selected video: " + fileName);

    /* You are accessing a dom element directly here, so you need to call "nativeElement" first. */
    this.myVideo.nativeElement.src = environment.apiUrl + "/Video/record/" + fileName;
    this.myVideo.nativeElement.play();
    this.videoName = fileName;
  }

  showQuestion(videoId: number) {
    var video = this.videos.find(v => v.videoId === videoId);
    console.log(video.question);

    //this.activeQuestion = video.question;

    this.playVideo(video.videoFileName)
  }
}

export interface IMedia {
  title: string;
  src: string;
  type: string;
}
