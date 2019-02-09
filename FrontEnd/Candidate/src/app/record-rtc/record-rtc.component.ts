import { Component, OnInit, ViewChild, AfterViewInit, ChangeDetectorRef } from '@angular/core';
let RecordRTC = require('recordrtc/RecordRTC.min');
import { HttpClient, HttpRequest, HttpEventType, HttpResponse } from '@angular/common/http';
import { Subscription } from 'rxjs';
import { environment } from '@environments/environment';
import { Router, ActivatedRoute } from '@angular/router';

import { RecordService, AuthenticationService } from '@app/_services';
import { User, Application, Question } from '@app/_models';

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

  private orderQuestions() {
    // Order array
    this.questions.sort((left, right): number => {
      if (left.order < right.order) return -1;
      if (left.order > right.order) return 1;
      return 0;
    })
  }

  upload(files) {
    if (files.length === 0)
      return;

    const formData = new FormData();

    for (let file of files)
      formData.append(file.name, file);

    this.baseUrl = environment.apiUrl + '/upload'; // WebAPI project
    console.log("BaseURL is: " + this.baseUrl);

    const uploadReq = new HttpRequest('POST', this.baseUrl, formData, {
      reportProgress: true,
    });

    this.http.request(uploadReq).subscribe(event => {
      if (event.type === HttpEventType.UploadProgress)
        this.progress = Math.round(100 * event.loaded / event.total);
      else if (event.type === HttpEventType.Response)
        this.message = event.body.toString();
    });
  }


  successCallback(stream: MediaStream) {

    var options = {
      mimeType: 'video/webm', // or video/webm\;codecs=h264 or video/webm\;codecs=vp9
      audioBitsPerSecond: 128000,
      videoBitsPerSecond: 128000,
      bitsPerSecond: 128000 // if this line is provided, skip above two
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

  processVideo(audioVideoWebMURL) {
    let video: HTMLVideoElement = this.video.nativeElement;
    let recordRTC = this.recordRTC;
    video.src = audioVideoWebMURL;
    this.toggleControls();
    var recordedBlob = recordRTC.getBlob();
    recordRTC.getDataURL(function (dataURL) { });
  }

  startRecording() {
    this.recordButtonDisabled = true;
    this.sendButtonDisabled = false;
    this.message = "Recording";

    let mediaConstraints = {
      video: true,
      audio: true
    };
    navigator.mediaDevices
      .getUserMedia(mediaConstraints)
      .then(this.successCallback.bind(this), this.errorCallback.bind(this));
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

  stopRecordingAndSend() {
    this.recordButtonDisabled = false;
    this.sendButtonDisabled = true;
    let recordRTC = this.recordRTC;
    //recordRTC.stopRecording(this.processVideo.bind(this));    

    var that = this;
    this.recordRTC.stopRecording(function () {
      //this.processVideo.bind(this)
      //that.recordedBlob = recordRTC.getBlob();
      //console.log("Blob size 1: " + that.recordedBlob.size);
      that.sendToServer(recordRTC.getBlob());
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
      console.log("Interview is completed");
      this.updateInterviewStatus(this.currentUser.applicationId, "Completed");
      this.router.navigate(["/home"]);
    }
    else {
      // Retrieve next question
      this.activeQuestion = this.questions[i + 1];
    }

  }

  updateInterviewStatus(applicationId: string, status: string){
    this.recordService.updateInterviewStatus(applicationId, status).subscribe(status => {

      console.log("Interview status was sucessfully updated");
    });
  }

  sendToServer(blob: Blob) {
    const formData = new FormData();

    var filename = this.currentTimeStamp() + ".webm";
    console.log(filename);

    formData.append('video-blob', blob, filename);
    console.log("Record blob size: " + blob.size);

    let baseUrl = environment.apiUrl + '/upload'; // WebAPI project
    console.log("BaseURL is: " + baseUrl);

    const uploadReq = new HttpRequest('POST', baseUrl, formData, {
      reportProgress: true,
    });

    //var that = this;
    this.http.request(uploadReq).subscribe(event => {
      if (event.type === HttpEventType.UploadProgress) {
        this.progress = Math.round(100 * event.loaded / event.total);
      }
      else if (event.type === HttpEventType.Response) {
        this.message = event.body.toString();
        this.ref.detectChanges(); // This is needed to update the view
      }
    });
  }

  download() {
    this.recordRTC.save('video.webm');
  }

  private currentTimeStamp(){
    return new Date().toISOString().replace(/:\s*/g, "");    
  }
}
