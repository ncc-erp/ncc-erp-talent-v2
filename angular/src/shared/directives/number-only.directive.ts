import { Directive, ElementRef, HostListener, Input } from '@angular/core';

@Directive({
  selector: '[numberOnly]',
})
export class NumberOnly {
    @Input() maxCharacter: number;
    public validKey: string[] = [
        '0',
        '1',
        '2',
        '3',
        '4',
        '5',
        '6',
        '7',
        '8',
        '9'
      ]

      private navigationKeys = [
        'Backspace',
        'Delete',
        'Tab',
        'Escape',
        'Enter',
        'Home',
        'End',
        'ArrowLeft',
        'ArrowRight',
        'Clear',
        'Copy',
      ];
    

  constructor(private _element: ElementRef) {
   }
   @HostListener('keydown', ['$event'])
  onKeyDown(e){
    const value = e.target.value.replace(/[^0-9]/g, '').slice(0, 10),
    isInvalidKey = !this.validKey.includes(e.key),
    isHavingEnoughNumbers = this.maxCharacter &&
        value.length === this.maxCharacter - 1 &&
        this.validKey.includes(e.key);
        if (
            this.navigationKeys.indexOf(e.key) > -1 ||
            (e.key === 'a' && e.ctrlKey === true) ||
            (e.key === 'c' && e.ctrlKey === true) ||
            (e.key === 'v' && e.ctrlKey === true) ||
            (e.key === 'x' && e.ctrlKey === true) 
          ) {
            return;
          }
        if(isInvalidKey || isHavingEnoughNumbers ){
            e.preventDefault();
            
        }
    }

    @HostListener('paste', ['$event']) onPaste(event: ClipboardEvent) {
        event.preventDefault();
        const pastedData = event.clipboardData?.getData('text');
        const numbersOnly = pastedData.replace(/[^0-9]/g, '').slice(0, 10);
        this._element.nativeElement.value = numbersOnly
      }
}
