import { Directive, ElementRef, HostListener, Input, OnInit } from '@angular/core';

@Directive({
    selector: '[textarearows]'
})

export class TextAreaRowsDirective implements OnInit {

    @Input('textarearows') text: string;

    constructor(
        private elr: ElementRef) {
    }

    ngOnInit(): any {
        var rows = this.text.split(/\r\n|\r|\n/).length;

        // Approximate height of the textarea
        this.elr.nativeElement.style.height = (rows + 1 + (rows * 0.5)) + "em";
    }

    /*
    @HostListener('mouseenter') onMouseEnter() {
        console.log(this.text);

        //reference to textarea
        this.elr.nativeElement.style.height = (this.elr.nativeElement.scrollHeight) + "px";
    }*/
}