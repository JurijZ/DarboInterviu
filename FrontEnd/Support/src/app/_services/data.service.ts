import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { InterviewTemplate, Interview } from '@app/_models';

@Injectable({ providedIn: 'root' })
export class DataService {
    
    private currentInterviewTemplateSubject: BehaviorSubject<InterviewTemplate> = new BehaviorSubject(new InterviewTemplate());
    public currentInterviewTemplate: Observable<InterviewTemplate> = this.currentInterviewTemplateSubject.asObservable();

    private currentInterviewSubject: BehaviorSubject<Interview> = new BehaviorSubject(new Interview());
    public currentInterview: Observable<Interview> = this.currentInterviewSubject.asObservable();

    constructor() {
    }

    // Storage of the currently selected InterviewTemplate
    public get currentInterviewTemplateValue(): InterviewTemplate {
        return this.currentInterviewTemplateSubject.value;
    }

    setInterviewTemplate(interview: InterviewTemplate) {
        this.currentInterviewTemplateSubject.next(interview);
    }

    clearInterviewTemplate() {
        this.currentInterviewTemplateSubject.next(null);
    }

    // Storage of the currently selected Interview
    public get currentInterviewValue(): Interview {
        return this.currentInterviewSubject.value;
    }

    setInterview(interview: Interview) {
        this.currentInterviewSubject.next(interview);
    }

    clearInterview() {
        this.currentInterviewSubject.next(null);
    }
}