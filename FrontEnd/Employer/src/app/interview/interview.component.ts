import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs';
import { first } from 'rxjs/operators';
import { Router, ActivatedRoute } from '@angular/router';
import { Interview } from '@app/_models';
import { InterviewService, DataService, AuthenticationService } from '@app/_services';

@Component({
  selector: 'app-interview',
  templateUrl: './interview.component.html',
  styleUrls: ['./interview.component.css']
})
export class InterviewComponent implements OnInit {
  interviews: Interview[] = [];

  constructor(
    private interviewService: InterviewService,
    private router: Router,
    private data: DataService) {

  }

  ngOnInit() {
    this.loadAllInterviews();
  }

  deleteInterview(id: string) {
    if (confirm("Are you sure you want to delete this Interview?")) {
      this.interviewService.delete(id).pipe(first()).subscribe(() => {
        this.loadAllInterviews()
      });
    }
  }

  private loadAllInterviews() {
    this.interviewService.getAll().pipe(first()).subscribe(interviews => {
      this.interviews = interviews;
    });
  }

  selectedInterview(interview: Interview) {
    this.data.setInterview(interview);
    this.router.navigate(['/question']);
  }
}
