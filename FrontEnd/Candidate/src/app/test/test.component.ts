let RecordRTC = require('recordrtc/RecordRTC.min');

import { Component, OnInit, ViewChild, AfterViewInit, ChangeDetectorRef, ElementRef, NgZone, OnDestroy } from '@angular/core';
import { HttpClient, HttpRequest, HttpEventType, HttpResponse } from '@angular/common/http';
import { Subscription } from 'rxjs';
import { environment } from '@environments/environment';
import { Router, ActivatedRoute } from '@angular/router';
import { TestService, AuthenticationService } from '@app/_services';
import { User, Application, Question } from '@app/_models';
import { DomSanitizer, SafeUrl } from '@angular/platform-browser';

@Component({
  selector: 'test',
  templateUrl: './test.component.html',
  styleUrls: ['./test.component.css']
})
export class TestComponent implements AfterViewInit, OnInit, OnDestroy {

  private stream: MediaStream;
  private recordRTC: any;
  public baseUrl: string;
  public progress: number;
  public message: string;
  public recordButtonDisabled: boolean = false;
  public sendButtonDisabled: boolean = true;
  public recordedBlob: Blob;
  public recordingState: boolean = true;
  public videoName: string;
  imageData: any;

  interval;
  timeLeft: number = 20; // seconds
  showTimer: boolean = false;

  activeQuestion: Question = new Question();
  questions: Question[];
  currentUser: User;
  currentUserSubscription: Subscription;

  @ViewChild('video') video;
  @ViewChild('myVideo') myVideo: any;

  constructor(
    private authenticationService: AuthenticationService,
    private http: HttpClient,
    private ref: ChangeDetectorRef,
    private testService: TestService,
    private router: Router,
    private elRef: ElementRef,
    private sanitizer: DomSanitizer,
    private ngZone: NgZone) {
    // Do stuff
  }

  ngOnInit() {
    this.currentUserSubscription = this.authenticationService.currentUser.subscribe(user => {
      this.currentUser = user;
      console.log(this.currentUser);
    });
  }

  ngOnDestroy() {
    // stop recording if it's still running
    if (this.recordButtonDisabled == true){
      this.stopRecording();
    }

    // unsubscribe to ensure no memory leaks
    this.currentUserSubscription.unsubscribe();
  }

  ngAfterViewInit() {
    // set the initial state of the video
    let video: HTMLVideoElement = this.video.nativeElement;
    video.muted = false;
    video.controls = true;
    video.autoplay = false;
  }  

  
  toggleControls() {
    let video: HTMLVideoElement = this.video.nativeElement;
    video.muted = !video.muted;
    video.controls = !video.controls;
    video.autoplay = !video.autoplay;
  }

  playVideo() {
    //console.log("Selected video: " + fileName);
    let fileName = "test.webm";

    /* You are accessing a dom element directly here, so you need to call "nativeElement" first. */
    this.myVideo.nativeElement.src = environment.apiUrl + "/Test/" + fileName;
    this.myVideo.nativeElement.play();
    this.videoName = fileName;
  } 

  enableRecordingState(){
    this.recordingState = true;    

    this.ref.detectChanges(); // This is needed to update the view otherwise the element does not yet exist   
    
    // set the initial state of the video
    let video: HTMLVideoElement = this.video.nativeElement;
    video.muted = false;
    video.controls = true;
    video.autoplay = false;

    this.recordButtonDisabled = false;
    this.sendButtonDisabled = true;
  }
  
  sendToServer(testId: string, blob: Blob) {
    this.testService.sendToServer(testId, blob).subscribe(event => {
      if (event.type === HttpEventType.UploadProgress) {
        this.progress = Math.round(100 * event.loaded / event.total);
      }
      else if (event.type === HttpEventType.Response) {
        this.message = event.body.toString();

        console.log("Playing back the video");
        this.recordingState = false;
        
        this.ref.detectChanges(); // This is needed to update the view otherwise the element does not yet exist   
        this.playVideo();   
      }      
    });
  }  

  // Linked to a button
  startRecording() {
    this.recordButtonDisabled = true;
    this.sendButtonDisabled = false;
    this.message = "Recording";
        
    // Timer
    this.timeLeft = 20; // Will stop the recording
    this.startTimer();

    let mediaConstraints = {
      video: true,
      audio: true
    };
    navigator.mediaDevices
      .getUserMedia(mediaConstraints)
      .then(this.successCallback.bind(this), this.errorCallback.bind(this));

    this.ref.detectChanges(); // This is needed to update the view otherwise the element does not yet exist
  }

  successCallback(stream: MediaStream) {
    var options = {
      type: 'video'
      //mimeType: 'video/webm', // or video/webm\;codecs=h264 or video/webm\;codecs=vp9
      //audioBitsPerSecond: 128000,
      //videoBitsPerSecond: 256000,
      //bitsPerSecond: 128000 // if this line is provided, skip above two
    };
    this.stream = stream;
    this.recordRTC = RecordRTC(stream, options);
    this.recordRTC.startRecording();
    let video: HTMLVideoElement = this.video.nativeElement;
    //video.src = window.URL.createObjectURL(stream); //createObjectURL() function is depricated in Chrome
    video.srcObject = stream;
    this.toggleControls();
  }

  errorCallback() {
    //handle error here
  }
  
  // Linked to a Button
  stopRecordingAndSend() {
    this.recordButtonDisabled = false;
    this.sendButtonDisabled = true;
    let recordRTC = this.recordRTC;
    let currentActiveQuestion = this.activeQuestion;
    //recordRTC.stopRecording(this.processVideo.bind(this)); 
    
    // Stop timer
    this.pauseTimer();

    var that = this;
    this.recordRTC.stopRecording(function () {
      //this.processVideo.bind(this)
      //that.recordedBlob = recordRTC.getBlob();
      //console.log("Id to be sent: " + that.activeQuestion.id);
      that.sendToServer("test", recordRTC.getBlob());      
    });
    let stream = this.stream;
    this.stream.getAudioTracks().forEach(track => track.stop());
    this.stream.getVideoTracks().forEach(track => track.stop());

    // reset the initial state of the video
    let video: HTMLVideoElement = this.video.nativeElement;
    video.muted = false;
    video.controls = true;
    video.autoplay = false;
  }

  // Called when the page is unexpectedly destroyed
  stopRecording() {
    this.recordButtonDisabled = false;
    this.sendButtonDisabled = true;
    let recordRTC = this.recordRTC;
    let currentActiveQuestion = this.activeQuestion;
    //recordRTC.stopRecording(this.processVideo.bind(this));    

    var that = this;
    this.recordRTC.stopRecording(function () {
      console.log("Recorded stopped unexpectedly");
      //that.sendToServer("test", recordRTC.getBlob());      
    });
    let stream = this.stream;
    this.stream.getAudioTracks().forEach(track => track.stop());
    this.stream.getVideoTracks().forEach(track => track.stop());
  }

  completeTest(){
    //- normal navigation does not work. Probably beacause of the this.ref.detectChanges(); usage
    //this.router.navigate(['/']); 
    
    this.ngZone.run(() => this.router.navigate(['/'])).then();
  }
  
  startTimer() {
    this.interval = setInterval(() => {
      // If you decide to change value 20 then do not forget to update html multiplication as well
      if (this.timeLeft > 0) {
        this.showTimer = true;
        this.timeLeft--;
        //console.log(this.timeLeft);
        this.ref.detectChanges(); // This is needed to update the view otherwise the element does not yet exist
      } else {
        // When time is up then stop recording and load next question
        this.showTimer = false;
        this.stopRecordingAndSend();
      }
    }, 1000)
  }

  pauseTimer() {
    clearInterval(this.interval);
  }
}
