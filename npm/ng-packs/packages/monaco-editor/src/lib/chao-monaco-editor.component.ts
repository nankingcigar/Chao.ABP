import {
  AfterViewInit,
  Component,
  ElementRef,
  forwardRef,
  Input,
  OnDestroy,
  OnInit,
  ViewChild,
  NgZone,
} from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import { fromEvent, Subscription } from 'rxjs';
import { EditorOptions } from './typings';

let loadedMonaco = false;
let loadPromise: Promise<void>;

declare var monaco: any;

@Component({
  selector: 'chao-monaco-editor',
  template: `<div class="editor-container" #editorContainer></div>`,
  styles: [
    `
      .editor-container {
        width: 100%;
        height: 30rem;
      }
    `,
  ],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => ChaoMonacoEditorComponent),
      multi: true,
    },
  ],
})
export class ChaoMonacoEditorComponent
  implements AfterViewInit, ControlValueAccessor, OnInit, OnDestroy
{
  @ViewChild('editorContainer', { static: true })
  editorContainer!: ElementRef;
  editor: any;

  _options!: EditorOptions;
  @Input('options')
  set options(value: EditorOptions) {
    this._options = value;
    if (this.editor) {
      this.editor.dispose();
      this.initMonaco(value);
    }
  }
  get options(): any {
    return this._options;
  }

  _value: string = '';
  propagateChange = (_: any) => {};
  onTouched = () => {};

  windowResizeSubscription!: Subscription;

  constructor(private zone: NgZone, private el: ElementRef) {}

  ngOnInit() {
    var nativeElement: HTMLElement = this.el.nativeElement,
      parentElement: HTMLElement = nativeElement.parentElement as HTMLElement;

    while (nativeElement.firstChild) {
      parentElement.insertBefore(nativeElement.firstChild, nativeElement);
    }
    parentElement.removeChild(nativeElement);
  }

  ngAfterViewInit(): void {
    if (loadedMonaco === true) {
      loadPromise.then(() => {
        this.initMonaco(this._options);
      });
    } else {
      loadedMonaco = true;
      loadPromise = new Promise<void>((resolve: any) => {
        const baseUrl = './assets';
        if (typeof (<any>window).monaco === 'object') {
          this.initMonaco(this._options);
          resolve();
          return;
        }
        const onGotAmdLoader: any = () => {
          (<any>window).require.config({
            paths: { vs: `${baseUrl}/monaco/min/vs` },
          });
          (<any>window).require([`vs/editor/editor.main`], () => {
            this.initMonaco(this._options);
            resolve();
          });
        };
        if (
          (<any>window).require === undefined ||
          (<any>window).require === null
        ) {
          const loaderScript: HTMLScriptElement =
            document.createElement('script');
          loaderScript.type = 'text/javascript';
          loaderScript.src = `${baseUrl}/monaco/min/vs/loader.js`;
          loaderScript.addEventListener('load', onGotAmdLoader);
          document.body.appendChild(loaderScript);
        } else {
          onGotAmdLoader();
        }
      });
    }
  }

  writeValue(value: any): void {
    this._value = value || '';
    if (this.editor && !this.options.model) {
      this.editor.setValue(this._value);
    }
  }

  registerOnChange(fn: any): void {
    this.propagateChange = fn;
  }

  registerOnTouched(fn: any): void {
    this.onTouched = fn;
  }

  protected initMonaco(options: EditorOptions): void {
    this.editor = monaco.editor.create(
      this.editorContainer.nativeElement,
      options
    );
    this.editor.setValue(this._value);
    this.editor.onDidChangeModelContent((e: any) => {
      const value = this.editor.getValue();
      this.propagateChange(value);
      this._value = value;
    });
    this.editor.onDidBlurEditorWidget(() => {
      this.onTouched();
    });
    if (this.windowResizeSubscription) {
      this.windowResizeSubscription.unsubscribe();
    }
    this.windowResizeSubscription = fromEvent(window, 'resize').subscribe(() =>
      this.editor.layout()
    );
  }

  ngOnDestroy() {
    if (this.windowResizeSubscription) {
      this.windowResizeSubscription.unsubscribe();
    }
    if (this.editor) {
      this.editor.dispose();
      this.editor = undefined;
    }
  }
}
