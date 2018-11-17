import { Component, Inject, OnInit, ViewChild } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-fetch-data',
  templateUrl: './fetch-data.component.html'
})
export class FetchDataComponent implements OnInit {
  public forecasts: WeatherForecast[];
  @ViewChild('myVideo') myVideo: any;
  public url: string;  
  sources: Array<Object>;
  
  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {    

    this.sources = [
      {
        src: "http://static.videogular.com/assets/videos/videogular.mp4",
        type: "video/mp4"
      },
      {
        src: "http://static.videogular.com/assets/videos/videogular.ogg",
        type: "video/ogg"
      },
      {
        src: "http://static.videogular.com/assets/videos/videogular.webm",
        type: "video/webm"
      }
    ];

    http.get<WeatherForecast[]>(baseUrl + 'api/SampleData/WeatherForecasts').subscribe(result => {
      this.forecasts = result;
      this.url = baseUrl + "api/Video/GetVideo";
    }, error => console.error(error));
  }

  ngOnInit() {
    console.log(this.url);
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
