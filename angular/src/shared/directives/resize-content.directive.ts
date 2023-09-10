import { Directive, ElementRef, HostListener, Input } from '@angular/core';

@Directive({
  selector: '[talentResizeContent]'
})
export class ResizeContentDirective {

  @Input() collapseLine: number = 1;

  constructor(public el: ElementRef) { }

  ngOnInit(): void { }

  @HostListener('click', ['$event'])
  handleKeyDown(event: KeyboardEvent) {
    this.el.nativeElement.classList.toggle(`max-line-content-${this.collapseLine}`);
  }
}
