import { Directive, ElementRef, Input, OnDestroy, Renderer2, TemplateRef, ViewContainerRef, OnInit } from '@angular/core';

@Directive({
  selector: '[tooltip]'
})
export class CustomTooltipDirective implements OnInit, OnDestroy {
  @Input('tooltip') tooltipContent!: string | TemplateRef<any>;

  private tooltipElement!: HTMLElement;
  private embeddedViewRef: any;

  constructor(
    private el: ElementRef,
    private renderer: Renderer2,
    private viewContainerRef: ViewContainerRef
  ) {}

  ngOnInit() {
    this.renderer.listen(this.el.nativeElement, 'mouseenter', () => this.showTooltip());
    this.renderer.listen(this.el.nativeElement, 'mouseleave', () => this.hideTooltip());
  }

  private showTooltip() {
    this.tooltipElement = this.renderer.createElement('div');
    this.renderer.addClass(this.tooltipElement, 'custom-tooltip');

    if (typeof this.tooltipContent === 'string') {
      this.renderer.setProperty(this.tooltipElement, 'textContent', this.tooltipContent);
    } else {
      this.embeddedViewRef = this.viewContainerRef.createEmbeddedView(this.tooltipContent);
      this.embeddedViewRef.rootNodes.forEach(node => {
        this.renderer.appendChild(this.tooltipElement, node);
      });
    }

    this.renderer.appendChild(document.body, this.tooltipElement);

    const { top, left, height } = this.el.nativeElement.getBoundingClientRect();
    this.renderer.setStyle(this.tooltipElement, 'top', `${top + height}px`);
    this.renderer.setStyle(this.tooltipElement, 'left', `${left}px`);
  }

  private hideTooltip() {
    if (this.tooltipElement) {
      this.renderer.removeChild(document.body, this.tooltipElement);
      if (this.embeddedViewRef) {
        this.embeddedViewRef.destroy();
        this.embeddedViewRef = null;
      }
    }
  }

  ngOnDestroy() {
    this.hideTooltip();
  }
}
