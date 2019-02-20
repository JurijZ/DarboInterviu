import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs';
import { first } from 'rxjs/operators';
import { Router, ActivatedRoute } from '@angular/router';
import { InterviewTemplate } from '@app/_models';
import { InterviewTemplateService, DataService, AuthenticationService } from '@app/_services';
import { User } from '@app/_models';

@Component({
  selector: 'app-interviewtemplate',
  templateUrl: './interviewtemplate.component.html',
  styleUrls: ['./interviewtemplate.component.css']
})
export class InterviewTemplateComponent implements OnInit {
  interviewTemplates: InterviewTemplate[] = [];
  currentUser: User;
  currentUserSubscription: Subscription;

  constructor(
    private interviewtemplateService: InterviewTemplateService,
    private router: Router,
    private data: DataService,
    private authenticationService: AuthenticationService) {
      this.currentUserSubscription = this.authenticationService.currentUser.subscribe(user => {
        this.currentUser = user;
        this.loadAllTemplates(this.currentUser.id);
      });
  }

  ngOnInit() {    
  }

  ngOnDestroy() {
    this.currentUserSubscription.unsubscribe();
  }

  private loadAllTemplates(userId: string) {
    this.interviewtemplateService.getAllByUserId(userId).pipe(first()).subscribe(interviewTemplates => {
      this.interviewTemplates = interviewTemplates;
    });
  }

  deleteInterview(id: string) {
    if (confirm("Are you sure you want to delete this Interview?")) {
      this.interviewtemplateService.delete(id).pipe(first()).subscribe(() => {
        this.loadAllTemplates(this.currentUser.id)
      });
    }
  }

  addInterview() {
    let newInterviewTemplate: InterviewTemplate = new InterviewTemplate();
    newInterviewTemplate.name = "";
    newInterviewTemplate.timestamp = "";
    newInterviewTemplate.userid = this.currentUser.id;

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
    this.loadAllTemplates(this.currentUser.id);
  }



  editInterview(interviewTemplate: InterviewTemplate) {
    this.data.setInterviewTemplate(interviewTemplate);
    this.router.navigate(['/question']);
  }
}
