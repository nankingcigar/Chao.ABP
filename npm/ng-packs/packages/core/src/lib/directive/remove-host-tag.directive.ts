import { Directive, ElementRef } from "@angular/core";

@Directive({
    selector: '[remove-host-tag]'
})
export class RemoveHostTagDirective {
    constructor(private el: ElementRef) {
    }

    ngOnInit() {
        const nativeElement: HTMLElement = this.el.nativeElement,
            hostElement: HTMLElement = nativeElement.parentElement as HTMLElement,
            parentElement: HTMLElement = hostElement.parentElement as HTMLElement;
        parentElement.insertBefore(nativeElement, hostElement);
        parentElement.removeChild(hostElement);
    }
}