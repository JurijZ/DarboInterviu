import { Component, Input, OnInit, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs';
import { first } from 'rxjs/operators';
import { Router, ActivatedRoute } from '@angular/router';
import { InterviewTemplate, Application, User } from '@app/_models';
import { InterviewTemplateService, DataService, AuthenticationService } from '@app/_services';
import { NgbModal, NgbDateStruct, NgbTimeStruct, ModalDismissReasons } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'interviewmodal',
  templateUrl: './interviewmodal.component.html',
  styleUrls: ['./interviewmodal.component.css']
})
export class InterviewModalComponent {
  interviews: InterviewTemplate[] = [];
  currentInterview: InterviewTemplate;
  closeResult: string;
  newApplication: Application = new Application;
  date: NgbDateStruct;
  minDate: NgbDateStruct;
  time: NgbTimeStruct;
  issent: boolean = false;
  currentUser: User;
  currentUserSubscription: Subscription;

  @Input() interview: InterviewTemplate;

  constructor(
    private authenticationService: AuthenticationService,
    private interviewTemplateService: InterviewTemplateService,
    private router: Router,
    private data: DataService,
    private modalService: NgbModal) {
      this.currentUserSubscription = this.authenticationService.currentUser.subscribe(user => {
        this.currentUser = user;
    });
  }

  ngOnInit() {
  }

  open(content) {
    // Populate properties
    this.currentInterview = this.interview;
    console.log(this.currentInterview.name);

    this.newApplication.templateId = this.currentInterview.id;
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
      // this result comes from the modal.close() in the html. Can be useful :)
      this.closeResult = `Closed with: ${result}`;

      // Cleanup variables of the modal window for the next application
      this.issent = false;
      this.newApplication.candidateEmail = "";
      this.newApplication.candidateName = "";
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
    this.newApplication.userId = this.currentUser.id;
    console.log(this.newApplication);

    var response = this.interviewTemplateService.createApplication(this.newApplication);

    response.subscribe(
      application => {
        console.log("POST was successful ", application);

        // If successfully submitted hide the Send button 
        this.issent = true;
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
