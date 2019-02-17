let RecordRTC = require('recordrtc/RecordRTC.min');

import { Component, OnInit, ViewChild, AfterViewInit, ChangeDetectorRef } from '@angular/core';
import { HttpClient, HttpRequest, HttpEventType, HttpResponse } from '@angular/common/http';
import { Subscription } from 'rxjs';
import { environment } from '@environments/environment';
import { Router, ActivatedRoute } from '@angular/router';
import { RecordService, AuthenticationService } from '@app/_services';
import { User, Application, Question } from '@app/_models';
import { NgbProgressbar } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'record-rtc',
  templateUrl: './record-rtc.component.html',
  styleUrls: ['./record-rtc.component.css']
})
export class RecordRTCComponent implements AfterViewInit, OnInit {

  private stream: MediaStream;
  private recordRTC: any;
  public baseUrl: string;
  public progress: number;
  public message: string;
  public recordButtonDisabled: boolean = false;
  public sendButtonDisabled: boolean = true;
  public recordedBlob: Blob;

  interval;
  timeLeft: number = 60;
  timeLeftMinutes: string;
  timeLeftMinutesInPercents: number;

  showTimer: boolean = false;
  activeQuestion: Question = new Question();
  questions: Question[];
  currentUser: User;
  currentUserSubscription: Subscription;

  @ViewChild('video') video;

  constructor(
    private authenticationService: AuthenticationService,
    private http: HttpClient,
    private ref: ChangeDetectorRef,
    private recordService: RecordService,
    private router: Router) {
    // Do stuff
  }

  ngOnInit() {
    this.currentUserSubscription = this.authenticationService.currentUser.subscribe(user => {
      this.currentUser = user;
      console.log(this.currentUser);

      this.recordService.getQuestionsByApplicationId(this.currentUser.applicationId).subscribe(questions => {
        this.questions = questions;
        this.orderQuestions();

        this.activeQuestion = this.questions[0];

        console.log("Current question:" + this.activeQuestion.text + ", order Id: " + this.activeQuestion.order);
      
        console.log("Start Recording")
        this.startRecording();
      });
    });
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
  
  sendToServer(applicationId: string, questionId: string, blob: Blob) {
    this.recordService.sendToServer(applicationId, questionId, blob).subscribe(event => {
      if (event.type === HttpEventType.UploadProgress) {
        this.progress = Math.round(100 * event.loaded / event.total);
      }
      else if (event.type === HttpEventType.Response) {
        this.message = event.body.toString();
        this.ref.detectChanges(); // This is needed to update the view
      }
    });
  }  

  // Linked to a button
  startRecording() {
    this.recordButtonDisabled = true;
    this.sendButtonDisabled = false;
    this.message = "Recording";

    // Timer
    this.timeLeft = this.activeQuestion.duration * 60;
    this.startTimer()

    let mediaConstraints = {
      video: true,
      audio: true
    };
    navigator.mediaDevices
      .getUserMedia(mediaConstraints)
      .then(this.successCallback.bind(this), this.errorCallback.bind(this));
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
      that.sendToServer(that.currentUser.applicationId, currentActiveQuestion.id, recordRTC.getBlob());
    });
    let stream = this.stream;
    this.stream.getAudioTracks().forEach(track => track.stop());
    this.stream.getVideoTracks().forEach(track => track.stop());

    // reset the initial state of the video
    let video: HTMLVideoElement = this.video.nativeElement;
    video.muted = false;
    video.controls = true;
    video.autoplay = false;

    // Show next question or complete
    var i = this.questions.findIndex(q => q.id === this.activeQuestion.id);
    if (i == this.questions.length - 1) {
      // Interview is completed
      console.log("Interview is Completed");
      this.updateInterviewStatus(this.currentUser.applicationId, "Completed");      
    }
    else {
      // Retrieve next question
      this.activeQuestion = this.questions[i + 1];
      console.log("Next question Id: " + this.activeQuestion.id);

      console.log("Continue Recording.");
      this.startRecording();
    }
  }

  updateInterviewStatus(applicationId: string, statusCode: string){
    this.recordService.updateInterviewStatus(applicationId, statusCode).subscribe(status => {
      console.log("Interview status was sucessfully updated");
      if (statusCode == "Completed"){
        //console.log("Redirecting");
        this.router.navigate(["/final"]);
      }
    });
  }

  private orderQuestions() {
    // Order array
    this.questions.sort((left, right): number => {
      if (left.order < right.order) return -1;
      if (left.order > right.order) return 1;
      return 0;
    })
  }

  startTimer() {
    this.interval = setInterval(() => {
      // If you decide to change value 20 then do not forget to update html multiplication as well
      if(this.timeLeft > 0 && this.timeLeft <= 25) {
        this.showTimer = true;
        this.timeLeft--;        
      } else if (this.timeLeft > 25) {
        this.showTimer = false;
        this.timeLeftMinutes = this.calculateMinutes(this.timeLeft - 1);
        this.timeLeftMinutesInPercents = this.calculateDurationPercentage(this.timeLeft - 1);
        this.timeLeft--;             
      } else {
        // When time is up then stop recording and load next question
        this.showTimer = false;
        this.stopRecordingAndSend();
      }
    },1000)
  }

  calculateMinutes(seconds: number){
    var m = Math.floor(seconds % 3600 / 60) + 1;
    var mDisplay = m > 0 ? m + (m == 1 ? " minute remain" : " minutes remain") : "";
    return mDisplay
  }

  calculateDurationPercentage(seconds: number){    
    var c = Math.floor(100 / (this.activeQuestion.duration));
    var m = Math.floor(seconds % 3600 / 60) + 1;
    var p = Math.floor(m * c);
    return p;
  }

  pauseTimer() {
    clearInterval(this.interval);
  }

  // NOT IN USE!!!
  processVideo(audioVideoWebMURL) {
    let video: HTMLVideoElement = this.video.nativeElement;
    let recordRTC = this.recordRTC;
    video.src = audioVideoWebMURL;
    this.toggleControls();
    var recordedBlob = recordRTC.getBlob();
    recordRTC.getDataURL(function (dataURL) { });
  }

  stopRecording() {
    this.recordButtonDisabled = false;
    this.sendButtonDisabled = true;
    let recordRTC = this.recordRTC;
    recordRTC.stopRecording(this.processVideo.bind(this));
    let stream = this.stream;
    stream.getAudioTracks().forEach(track => track.stop());
    stream.getVideoTracks().forEach(track => track.stop());
  }

  
  upload(files) {
    this.recordService.upload(files).subscribe(event => {
      if (event.type === HttpEventType.UploadProgress)
        this.progress = Math.round(100 * event.loaded / event.total);
      else if (event.type === HttpEventType.Response)
        this.message = event.body.toString();
    });
  }
  
}
