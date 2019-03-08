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
  loading: boolean = false;
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
        this.loadAllInterviews(this.currentUser.id);
      });
  }

  ngOnInit() {    
  }

  ngOnDestroy() {
    this.currentUserSubscription.unsubscribe();
  }

  refreshInterviews(userId: string) {
    this.loadAllInterviews(userId);
  }

  private loadAllInterviews(userId: string) {
    this.loading = true; //show the spinner

    this.interviewService.getAllByUserId(userId).pipe(first()).subscribe(interviews => {
      this.interviews = interviews;
      this.loading = false; //hide the spinner
    });
  }

  reviewInterview(interview: Interview) {
    this.data.setInterview(interview);
    this.router.navigate(['/review']);
  }

  isButtonDisabled(status: string){
    if (status == 'NotStarted'){
      return true;
    }
    return false;
  }
  
  translateStatus(status: string){
    if(status === 'NotStarted'){
      return 'Nėra Pradėtas';
    }
    else if(status === 'Completed'){
      return 'Užbaigtas';
    }
    else if(status === 'Expired'){
      return 'Nebuvo Atliktas';
    }
    else {
      return 'Nežinoma';
    }
  }
}
