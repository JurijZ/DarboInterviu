import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { Question } from '@app/_models';

@Injectable({ providedIn: 'root' })
export class DataService {
    
    private currentQuestionSubject: BehaviorSubject<Question> = new BehaviorSubject(new Question());
    public currentQuestion: Observable<Question> = this.currentQuestionSubject.asObservable();

    constructor() {
    }

    public get currentQuestionValue(): Question {
        return this.currentQuestionSubject.value;
    }

    setQuestion(question: Question) {
        this.currentQuestionSubject.next(question);
    }

    clearQuestion() {
        this.currentQuestionSubject.next(null);
    }
}