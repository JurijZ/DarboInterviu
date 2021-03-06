import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs';
import { first } from 'rxjs/operators';
import { Router, ActivatedRoute } from '@angular/router';
import { Interview, User } from '@app/_models';
import { InterviewService, DataService, AuthenticationService } from '@app/_services';

@Component({
  selector: 'app-interview',
  templateUrl: './interview.component.html',
  styleUrls: ['./interview.component.css']
})
export class InterviewComponent implements OnInit, OnDestroy {
  interviews: Interview[] = [];
  currentUser: User;
  currentUserSubscription: Subscription;

  constructor(
    private authenticationService: AuthenticationService,
    private interviewService: InterviewService,
    private router: Router,
    private data: DataService) {
      this.currentUserSubscription = this.authenticationService.currentUser.subscribe(user => {
        this.currentUser = user;
        this.loadAllInterviews();
      });
  }

  ngOnInit() {    
  }

  ngOnDestroy() {
    this.currentUserSubscription.unsubscribe();
  }

  refreshInterviews() {
    this.loadAllInterviews();
  }

  private loadAllInterviews() {
    this.interviewService.getAll().pipe(first()).subscribe(interviews => {
      this.interviews = interviews;
    });
  }

  reviewInterview(interview: Interview) {
    this.data.setInterview(interview);
    this.router.navigate(['/review']);
  }

  isButtonDisabled(status: string){
    if (status == 'Not Started'){
      return true;
    }
    return false;
  }
}
