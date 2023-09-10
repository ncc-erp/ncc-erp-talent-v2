import { Directive, ElementRef, Input, OnInit } from '@angular/core';

@Directive({
  selector: '[talentAutoFocus]'
})
export class AutoFocusDirective implements OnInit{

  constructor(public el: ElementRef) { }

  ngOnInit(): void {
    this.el.nativeElement.focus();
  }
}
