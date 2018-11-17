import { Component, Inject, OnInit, ViewChild } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-fetch-data',
  templateUrl: './videos.component.html'
})
export class VideosComponent implements OnInit {
  public forecasts: WeatherForecast[];
  @ViewChild('myVideo') myVideo: any;
  public url: string;
  public baseUrl: string;
  
  constructor(http: HttpClient) {    
    
    this.baseUrl = document.getElementsByTagName('base')[0].href;

    http.get<WeatherForecast[]>(this.baseUrl + 'api/SampleData/WeatherForecasts').subscribe(result => {
      this.forecasts = result;
      this.url = this.baseUrl + "api/Video/GetVideo";
    }, error => console.error(error));
  }

  ngOnInit() {
    console.log("BaseURL is: " + this.baseUrl);
  }

  playVideo() {
    /**
     * You are accessing a dom element directly here,
     * so you need to call "nativeElement" first.
     */
    console.log("Click");
    console.log(this.url);

    this.myVideo.nativeElement.play();
  }

 
}

interface WeatherForecast {
  dateFormatted: string;
  temperatureC: number;
  temperatureF: number;
  summary: string;
}

export interface IMedia {
  title: string;
  src: string;
  type: string;
}
