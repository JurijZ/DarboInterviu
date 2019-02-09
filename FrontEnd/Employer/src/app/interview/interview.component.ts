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
export class InterviewComponent implements OnInit {
  interviews: Interview[] = [];
  currentUser: User;
  currentUserSubscription: Subscription;

  constructor(
    private authenticationService: AuthenticationService,
    private interviewService: InterviewService,
    private router: Router,
    private data: DataService) {
      
  }

  ngOnInit() {
    this.currentUserSubscription = this.authenticationService.currentUser.subscribe(user => {
      this.currentUser = user; 
      this.loadAllInterviews(this.currentUser.id);
    });
    
  }

  refreshInterviews(userId: string) {
    this.loadAllInterviews(userId);
  }

  private loadAllInterviews(userId: string) {
    this.interviewService.getAll(userId).pipe(first()).subscribe(interviews => {
      this.interviews = interviews;
    });
  }

}
