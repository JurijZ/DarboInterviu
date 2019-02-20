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
  interviewTemplates: InterviewTemplate[] = [];

  constructor(
    private interviewtemplateService: InterviewTemplateService,
    private router: Router,
    private data: DataService) {

  }

  ngOnInit() {
    this.loadAllTemplates();
  }

  private loadAllTemplates() {
    this.interviewtemplateService.getAll().pipe(first()).subscribe(interviewTemplates => {
      this.interviewTemplates = interviewTemplates;
    });
  }

  deleteInterview(id: string) {
    if (confirm("Are you sure you want to delete this Interview?")) {
      this.interviewtemplateService.delete(id).pipe(first()).subscribe(() => {
        this.loadAllTemplates()
      });
    }
  }

  addInterview() {
    let newInterviewTemplate: InterviewTemplate = new InterviewTemplate();
    newInterviewTemplate.name = "";
    newInterviewTemplate.timestamp = "";

    //this.questions.push(newQuestion);

    var response = this.interviewtemplateService.create(newInterviewTemplate);

    response.subscribe(
      interviewTemplate => {
        console.log("POST was successful ", interviewTemplate.id);
        this.data.setInterviewTemplate(interviewTemplate);
        this.router.navigate(['/question']);
        //this.loadAllInterviews();
      },
      error => {
        console.log("Error: ", error);
      }
    );
  }

  refreshInterviews() {
    this.loadAllTemplates();
  }



  editInterview(interviewTemplate: InterviewTemplate) {
    this.data.setInterviewTemplate(interviewTemplate);
    this.router.navigate(['/question']);
  }
}
