import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs';
import { first } from 'rxjs/operators';

import { Interview, Question } from '@app/_models';
import { InterviewService, QuestionService, AuthenticationService } from '@app/_services';

@Component({
  selector: 'app-question',
  templateUrl: './question.component.html',
  styleUrls: ['./question.component.css']
})
export class QuestionComponent implements OnInit {
  questions: Question[] = [];
  interview: Interview;

  constructor(private questionService: QuestionService) {

  }

  ngOnInit() {
    this.loadAllQuestions();
  }

  deleteInterview(id: string) {
    this.questionService.delete(id).pipe(first()).subscribe(() => {
      this.loadAllQuestions()
    });
  }

  private loadAllQuestions() {
    this.questionService.getAll().pipe(first()).subscribe(questions => {
      this.questions = questions;
    });
  }

}
