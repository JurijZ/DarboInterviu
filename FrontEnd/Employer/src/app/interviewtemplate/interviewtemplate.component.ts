import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs';
import { first } from 'rxjs/operators';
import { Router, ActivatedRoute } from '@angular/router';
import { InterviewTemplate } from '@app/_models';
import { InterviewTemplateService, DataService, AuthenticationService } from '@app/_services';

@Component({
  selector: 'app-interviewtemplate',
  templateUrl: './interviewtemplate.component.html',
  styleUrls: ['./interviewtemplate.component.css']
})
export class InterviewTemplateComponent implements OnInit {
  interviews: InterviewTemplate[] = [];
  newInterview: InterviewTemplate;

  constructor(
    private interviewtemplateService: InterviewTemplateService,
    private router: Router,
    private data: DataService) {

  }

  ngOnInit() {
    this.loadAllInterviews();
  }

  deleteInterview(id: string) {
    if (confirm("Are you sure you want to delete this Interview?")) {
      this.interviewtemplateService.delete(id).pipe(first()).subscribe(() => {
        this.loadAllInterviews()
      });
    }
  }

  addInterview() {
    let newInterview: InterviewTemplate = new InterviewTemplate();
    newInterview.name = "";
    newInterview.timestamp = "";

    //this.questions.push(newQuestion);

    var response = this.interviewtemplateService.create(newInterview);

    response.subscribe(
      interview => {
        console.log("POST was successful ", interview.id);
        this.data.setInterview(interview);
        this.router.navigate(['/question']);
        //this.loadAllInterviews();
      },
      error => {
        console.log("Error: ", error);
      }
    );
  }

  refreshInterviews() {
    this.loadAllInterviews();
  }

  private loadAllInterviews() {
    this.interviewtemplateService.getAll().pipe(first()).subscribe(interviews => {
      this.interviews = interviews;
    });
  }

  editInterview(interview: InterviewTemplate) {
    this.data.setInterview(interview);
    this.router.navigate(['/question']);
  }
}
