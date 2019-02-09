import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { InterviewTemplate } from '@app/_models';

@Injectable({ providedIn: 'root' })
export class DataService {
    
    private currentInterviewSubject: BehaviorSubject<InterviewTemplate> = new BehaviorSubject(new InterviewTemplate());
    public currentInterview: Observable<InterviewTemplate> = this.currentInterviewSubject.asObservable();

    constructor() {
    }

    public get currentInterviewValue(): InterviewTemplate {
        return this.currentInterviewSubject.value;
    }

    setInterview(interview: InterviewTemplate) {
        this.currentInterviewSubject.next(interview);
    }

    clearInterview() {
        this.currentInterviewSubject.next(null);
    }
}