import { Component, Input, OnInit, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs';
import { first } from 'rxjs/operators';
import { Router, ActivatedRoute } from '@angular/router';
import { Interview, Application } from '@app/_models';
import { InterviewService, DataService, AuthenticationService } from '@app/_services';
import { NgbModal, NgbDateStruct, NgbTimeStruct, ModalDismissReasons } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'interviewmodal',
  templateUrl: './interviewmodal.component.html',
  styleUrls: ['./interviewmodal.component.css']
})
export class InterviewModalComponent {
  interviews: Interview[] = [];
  currentInterview: Interview;
  closeResult: string;
  newApplication: Application = new Application;
  date: NgbDateStruct;
  minDate: NgbDateStruct;
  time: NgbTimeStruct;

  @Input() interview: Interview;

  constructor(
    private interviewService: InterviewService,
    private router: Router,
    private data: DataService,
    private modalService: NgbModal) {
  }

  ngOnInit() {
  }

  open(content) {
    // Populate properties
    this.currentInterview = this.interview;
    console.log(this.currentInterview.name);

    this.newApplication.interviewId = this.currentInterview.id;
    this.newApplication.title = this.currentInterview.name;

    // Set min selectable date
    let currentDate2 = new Date();
    currentDate2.setDate(currentDate2.getDate() + 2);
    this.minDate = {
      year: currentDate2.getFullYear(),
      month: currentDate2.getMonth() + 1,
      day: currentDate2.getDate()
    };

    // Add 2 days
    let currentDate = new Date();
    currentDate.setDate(currentDate.getDate() + 2);
    console.log(currentDate);

    this.date = {
      year: currentDate.getFullYear(),
      month: currentDate.getMonth() + 1,
      day: currentDate.getDate()
    };
    this.time = { hour: 23, minute: 0, second: 0 };

    //
    this.modalService.open(content, { ariaLabelledBy: 'modal-basic-title' }).result.then((result) => {
      this.closeResult = `Closed with: ${result}`;
      console.log(this.newApplication.title);
      console.log(this.date);
      console.log(this.time);
      // Call Web API POST
    }, (reason) => {
      this.closeResult = `Dismissed ${this.getDismissReason(reason)}`;
    });
  }

  private getDismissReason(reason: any): string {
    if (reason === ModalDismissReasons.ESC) {
      return 'by pressing ESC';
    } else if (reason === ModalDismissReasons.BACKDROP_CLICK) {
      return 'by clicking on a backdrop';
    } else {
      return `with: ${reason}`;
    }
  }

  createApplicant() {
    this.newApplication.expiration = this.formatDateTime(this.date, this.time);
    console.log(this.newApplication);

    var response = this.interviewService.createApplication(this.newApplication);

    response.subscribe(
      application => {
        console.log("POST was successful ", application);
        //this.data.setInterview(interview);
        //this.router.navigate(['/question']);
        //this.loadAllInterviews();
      },
      error => {
        console.log("Error: ", error);
      }
    );
  }

  formatDateTime(date: NgbDateStruct, time: NgbTimeStruct): string {
    return date.year + "-" + this.addLeadingZero(date.month) + "-" + this.addLeadingZero(date.day) + "T" +
      this.addLeadingZero(time.hour) + ":" + this.addLeadingZero(time.minute) + ":01.001Z";

    //return "2019-02-05T14:06:39.536Z"
  }

  addLeadingZero(n: number): string {
    if (n < 10) {
      return "0" + n;
    } else {
      return n.toString();
    }
  }
}
