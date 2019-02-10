import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs';
import { first } from 'rxjs/operators';

import { InterviewTemplate, Question, User } from '@app/_models';
import { DataService, QuestionService, AuthenticationService } from '@app/_services';
import { loadQueryList } from '@angular/core/src/render3';

@Component({
  selector: 'app-question',
  templateUrl: './question.component.html',
  styleUrls: ['./question.component.css']
})
export class QuestionComponent implements OnInit {
  questions: Question[] = [];
  interviewTemplate: InterviewTemplate;
  durations: number[] = [5, 10, 15, 20];
  currentUser: User;
  currentUserSubscription: Subscription;

  constructor(
    private authenticationService: AuthenticationService,
    private questionService: QuestionService, 
    private data: DataService
    ) {
      this.currentUserSubscription = this.authenticationService.currentUser.subscribe(user => {
        this.currentUser = user;
      });
  }

  ngOnInit() {
    this.data.currentInterviewTemplate.subscribe(interviewTemplate => {
      this.interviewTemplate = interviewTemplate;
      this.loadAllQuestions(this.interviewTemplate.id);
    })
  }

  deleteQuestion(id: string) {
    if (confirm("Are you sure you want to delete this question?")) {
      this.questionService.delete(id).pipe(first()).subscribe(() => {
        this.loadAllQuestions(this.interviewTemplate.id)
      });
    }
  }

  //selectDuration(duration: number){  }

  addQuestion() {
    let newQuestion: Question = new Question();
    //newQuestion.id = 1;
    newQuestion.duration = 5;
    newQuestion.order = (this.questions.length == 0) ? 0 : this.questions[this.questions.length - 1].order + 1;
    newQuestion.text = "";
    newQuestion.interview = this.interviewTemplate.id;

    //this.questions.push(newQuestion);

    var response = this.questionService.create(newQuestion);

    response.subscribe(
      data => {
        console.log("POST was successful ", data);
        this.loadAllQuestions(this.interviewTemplate.id);
      },
      error => {
        console.log("Error: ", error);
      }
    );
  }

  refreshQuestions() {
    this.loadAllQuestions(this.interviewTemplate.id);
  }

  updateQuestion(question: Question) {
    var c = this.questionService.update(question);
  }

  updateInterview(){
    this.interviewTemplate.userid = this.currentUser.id;
    this.questionService.updateInterview(this.interviewTemplate).subscribe(
      data => {
        console.log("Interview update was successful ", data);
      },
      error => {
        console.log("Error in Interview update: ", error);
      });
  }

  private loadAllQuestions(interviewId: string) {
    this.questionService.getByInterviewId(interviewId).pipe(first()).subscribe(questions => {
      this.questions = questions;

      this.orderQuestions();
    });
  }

  up(index: number) {
    if (index > 0) {
      console.log(index);

      var temp = this.questions[index];

      this.questions[index] = this.questions[index - 1];
      this.questions[index].order = index;
      this.updateQuestion(this.questions[index]);

      this.questions[index - 1] = temp;
      this.questions[index - 1].order = index - 1;
      this.updateQuestion(this.questions[index - 1]);
    }
  }

  down(index: number) {
    if (index < this.questions.length - 1) {
      console.log(index);

      var temp = this.questions[index];

      this.questions[index] = this.questions[index + 1];
      this.questions[index].order = index;
      this.updateQuestion(this.questions[index]);

      this.questions[index + 1] = temp;
      this.questions[index + 1].order = index + 1;
      this.updateQuestion(this.questions[index + 1]);
    }
  }

  private orderQuestions() {
    // Order array
    this.questions.sort((left, right): number => {
      if (left.order < right.order) return -1;
      if (left.order > right.order) return 1;
      return 0;
    })
  }
}
