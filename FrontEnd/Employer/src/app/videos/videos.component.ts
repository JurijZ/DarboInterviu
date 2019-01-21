import { Component, Inject, OnInit, ViewChild } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '@environments/environment';

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
  
  constructor(http: HttpClient) {
    this.baseUrl = environment.apiUrl + '/Video'; // WebAPI project - visi video 
    console.log("BaseURL is: " + this.baseUrl);
    //this.baseUrl = document.getElementsByTagName('base')[0].href; //Angular project
    
    http.get<Video[]>(this.baseUrl).subscribe(result => {
      this.videos = result;
    }, error => console.error(error));
  }

  ngOnInit() {
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
