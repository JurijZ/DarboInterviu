import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { Interview } from '@app/_models';

@Injectable({ providedIn: 'root' })
export class DataService {
    
    private currentInterviewSubject: BehaviorSubject<Interview> = new BehaviorSubject(new Interview());
    public currentInterview: Observable<Interview> = this.currentInterviewSubject.asObservable();

    constructor() {
    }

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