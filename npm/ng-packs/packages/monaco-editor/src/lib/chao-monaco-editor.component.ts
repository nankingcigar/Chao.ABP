import {
  AfterViewInit,
  Component,
  ElementRef,
  forwardRef,
  Input,
  OnDestroy,
  ViewChild,
  NgZone
} from '@angular/core';
import { ControlValueAccessor, FormsModule, NG_VALUE_ACCESSOR } from '@angular/forms';
import { fromEvent, Subscription } from 'rxjs';
import { debounceTime } from 'rxjs/operators';
import { EditorOptions, IStandaloneCodeEditor } from './typings';
import { ChaoCoreModule } from '@nankingcigar/ng.core';

let monacoLoadPromise: Promise<void> | null = null;

/**
 * 核心逻辑：直接通过 域名 + baseHref 拼接绝对路径
 */
function getAbsoluteBaseUrl(): string {
  const baseHref = document.querySelector('base')?.getAttribute('href') || '/';
  // 确保 baseHref 前后斜杠闭合，避免拼接时出现 // 或漏掉 /
  const cleanBase = baseHref.endsWith('/') ? baseHref : baseHref + '/';
  const cleanBaseStart = cleanBase.startsWith('/') ? cleanBase : '/' + cleanBase;
  
  return `${window.location.origin}${cleanBaseStart}assets`;
}

const ensureMonacoLoaded = (language: string): Promise<void> => {
  if (monacoLoadPromise) return monacoLoadPromise;

  monacoLoadPromise = new Promise<void>((resolve) => {
    if ((<any>window).monaco) {
      resolve();
      return;
    }

    const absoluteAssetsUrl = getAbsoluteBaseUrl(); 
    
    // 拆分出 base 路径和 vs 路径
    const monacoBasePath = `${absoluteAssetsUrl}/monaco/min`;
    const monacoVsPath = `${monacoBasePath}/vs`;

    (window as any).MonacoEnvironment = {
      getWorkerUrl: function (moduleId: string, label: string) {
        const workerCode = `
          // 关键修正：Worker 内部拼接模块时自带 'vs/' 前缀，所以这里的 baseUrl 必须是 vs 的父级目录
          self.MonacoEnvironment = { baseUrl: '${monacoBasePath}' };
          importScripts('${monacoVsPath}/base/worker/workerMain.js');
        `;
        const blob = new Blob([workerCode], { type: 'application/javascript' });
        return URL.createObjectURL(blob);
      }
    };

    const onGotAmdLoader = () => {
      (<any>window).require.config({
        paths: { vs: monacoVsPath }, // 主线程的 require 还是需要指向 vs
      });
      (<any>window).require.config({
        'vs/nls': { availableLanguages: { '*': language } },
      });
      (<any>window).require([`vs/editor/editor.main`], () => {
        resolve();
      });
    };

    if (!(<any>window).require) {
      const loaderScript = document.createElement('script');
      loaderScript.type = 'text/javascript';
      loaderScript.src = `${monacoVsPath}/loader.js`;
      loaderScript.addEventListener('load', onGotAmdLoader);
      document.body.appendChild(loaderScript);
    } else {
      onGotAmdLoader();
    }
  });

  return monacoLoadPromise;
};

@Component({
  imports: [FormsModule, ChaoCoreModule],
  selector: 'chao-monaco-editor',
  template: `<div remove-host-tag class="chao-monaco-editor" #editorContainer style="height: 100%; width: 100%;"></div>`,
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => ChaoMonacoEditorComponent),
      multi: true,
    }
  ]
})
export class ChaoMonacoEditorComponent implements AfterViewInit, ControlValueAccessor, OnDestroy {
  @ViewChild('editorContainer', { static: true }) editorContainer!: ElementRef;
  
  editor!: IStandaloneCodeEditor;
  private _options!: EditorOptions;
  private _value: string = '';
  private windowResizeSubscription!: Subscription;

  propagateChange = (_: any) => { };
  onTouched = () => { };

  @Input() language: 'de' | 'es' | 'fr' | 'it' | 'ja' | 'ko' | 'ru' | 'zh-cn' | 'zh-tw' = 'zh-cn';

  @Input('options')
  set options(value: EditorOptions) {
    this._options = value;
    if (this.editor) {
      this.editor.updateOptions(value);
    }
  }
  get options(): any { return this._options; }

  constructor(private ngZone: NgZone) { }

  async ngAfterViewInit(): Promise<void> {
    await ensureMonacoLoaded(this.language);
    this.initMonaco(this._options);
  }

  protected initMonaco(options: EditorOptions): void {
    this.ngZone.runOutsideAngular(() => {
      this.editor = (window as any).monaco.editor.create(
        this.editorContainer.nativeElement,
        options
      );
      
      this.editor.setValue(this._value);

      this.editor.onDidChangeModelContent(() => {
        const value = this.editor.getValue();
        this._value = value;
        this.propagateChange(value);
      });

      this.editor.onDidBlurEditorWidget(() => {
        this.onTouched();
      });

      if (this.windowResizeSubscription) {
        this.windowResizeSubscription.unsubscribe();
      }

      this.windowResizeSubscription = fromEvent(window, 'resize')
        .pipe(debounceTime(150))
        .subscribe(() => this.editor.layout());
    });
  }

  writeValue(value: any): void {
    this._value = value || '';
    if (this.editor) {
      this.editor.setValue(this._value);
    }
  }

  registerOnChange(fn: any): void { this.propagateChange = fn; }
  registerOnTouched(fn: any): void { this.onTouched = fn; }

  ngOnDestroy() {
    if (this.windowResizeSubscription) this.windowResizeSubscription.unsubscribe();
    if (this.editor) {
      this.editor.dispose();
      this.editor = undefined as any;
    }
  }
}