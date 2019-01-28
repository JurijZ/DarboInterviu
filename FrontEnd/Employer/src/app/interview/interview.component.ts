import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs';
import { first } from 'rxjs/operators';

import { Interview } from '@app/_models';
import { InterviewService, AuthenticationService } from '@app/_services';

@Component({
  selector: 'app-interview',
  templateUrl: './interview.component.html',
  styleUrls: ['./interview.component.css']
})
export class InterviewComponent implements OnInit {
  interviews: Interview[] = [];

  constructor(private interviewService: InterviewService) {
    
  }

ngOnInit() {
  this.loadAllInterviews();
}

  deleteInterview(id: string) {
    this.interviewService.delete(id).pipe(first()).subscribe(() => {
      this.loadAllInterviews()
    });
  }

private loadAllInterviews() {
  this.interviewService.getAll().pipe(first()).subscribe(interviews => {
    this.interviews = interviews;
  });
}

}
