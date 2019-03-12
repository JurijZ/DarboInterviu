import { Component, Input, OnInit, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs';
import { first } from 'rxjs/operators';
import { Router, ActivatedRoute } from '@angular/router';
import { Interview, Application, User, Share } from '@app/_models';
import { InterviewService, DataService, AuthenticationService } from '@app/_services';
import { NgbModal, NgbDateStruct, NgbTimeStruct, ModalDismissReasons } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'interviewshare',
  templateUrl: './interviewshare.component.html',
  styleUrls: ['./interviewshare.component.css']
})
export class InterviewShareComponent {
  currentInterview: Interview;
  closeResult: string;
  newShare: Share = new Share;
  newApplication: Application = new Application;
  date: NgbDateStruct;
  minDate: NgbDateStruct;
  time: NgbTimeStruct;
  issent: boolean = false;
  currentUser: User;
  currentUserSubscription: Subscription;

  @Input() interview: Interview;

  constructor(
    private authenticationService: AuthenticationService,
    private interviewService: InterviewService,
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
    console.log(this.currentInterview.title);

    this.newShare.applicationId = this.currentInterview.id;
    this.newShare.userId = this.currentUser.id;

    //
    this.modalService.open(content, { ariaLabelledBy: 'modal-basic-title' }).result.then((result) => {
      // this result comes from the modal.close() in the html. Can be useful :)
      this.closeResult = `Closed with: ${result}`;

      // Cleanup variables of the modal window for the next application
      this.issent = false;
      this.newShare.email = "";
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

  shareInterview() {
    console.log(this.newShare);

    var response = this.interviewService.shareInterview(this.newShare);

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
}
