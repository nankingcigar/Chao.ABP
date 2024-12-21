import { AfterViewInit, Component, ElementRef, forwardRef, Input, OnDestroy, ViewChild } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import { createEditor, createToolbar, IDomEditor, IEditorConfig, IToolbarConfig, Toolbar, i18nChangeLanguage } from '@wangeditor/editor';

@Component({
  selector: 'chao-wang-editor',
  template: `
  <div remove-host-tag class="chao-wang-editor">
    <div class="chao-wang-editor-toolbar" #editorToolbar>
    </div>
    <div class="chao-wang-editor-container" #editorContainer>
    </div>
  </div>
  `,
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => ChaoWangEditorComponent),
      multi: true,
    }
  ],
  standalone: false
})
export class ChaoWangEditorComponent implements AfterViewInit, ControlValueAccessor, OnDestroy {
  @ViewChild('editorContainer', { static: true })
  editorContainer!: ElementRef;
  @ViewChild('editorToolbar', { static: true })
  editorToolbar!: ElementRef;
  editor!: IDomEditor;
  toolbar!: Toolbar;

  defaultEditorConfig = {
    autoFocus: false,
    MENU_CONF: {
      uploadImage: {
        base64LimitSize: 10 * 1024 * 1024,
        maxFileSize: 10 * 1024 * 1024
      }
    },
    hoverbarKeys: {
      'link': {
        'menuKeys': [
          'editLink',
          'unLink',
          'viewLink'
        ]
      },
      'image': {
        'menuKeys': [
          'imageWidth30',
          'imageWidth50',
          'imageWidth100',
          'editImage',
          'viewImageLink',
          'deleteImage'
        ]
      },
      'table': {
        'menuKeys': [
          'enter',
          'tableHeader',
          'tableFullWidth',
          'insertTableRow',
          'deleteTableRow',
          'insertTableCol',
          'deleteTableCol',
          'deleteTable'
        ]
      },
      'text': {
        'menuKeys': [
          'headerSelect',
          'insertLink',
          'bulletedList',
          'bold',
          'underline',
          'italic',
          'through',
          'clearStyle'
        ]
      },
      'formula': {
        'menuKeys': [
          'editFormula'
        ]
      }
    },
    onChange: () => {
      const value = this.editor.getHtml();
      this.propagateChange(value);
      this._value = value;
    },
    onBlur: () => {
      this.onTouched();
    }
  };

  defaultToolbarConfig = {
    toolbarKeys: [
      'headerSelect',
      'bold',
      'underline',
      'italic',
      'through',
      'clearStyle',
      'bulletedList',
      'numberedList',
      {
        'key': 'group-justify',
        'title': '对齐',
        'iconSvg': '<svg viewBox=\'0 0 1024 1024\'><path d=\'M768 793.6v102.4H51.2v-102.4h716.8z m204.8-230.4v102.4H51.2v-102.4h921.6z m-204.8-230.4v102.4H51.2v-102.4h716.8zM972.8 102.4v102.4H51.2V102.4h921.6z\'></path></svg>',
        'menuKeys': [
          'justifyLeft',
          'justifyRight',
          'justifyCenter',
          'justifyJustify'
        ]
      },
      {
        'key': 'group-indent',
        'title': '缩进',
        'iconSvg': '<svg viewBox=\'0 0 1024 1024\'><path d=\'M0 64h1024v128H0z m384 192h640v128H384z m0 192h640v128H384z m0 192h640v128H384zM0 832h1024v128H0z m0-128V320l256 192z\'></path></svg>',
        'menuKeys': [
          'indent',
          'delIndent'
        ]
      },
      'insertLink',
      'insertTable',
      'insertFormula',
      {
        'key': 'group-image',
        'title': '图片',
        'iconSvg': '<svg viewBox=\'0 0 1024 1024\'><path d=\'M959.877 128l0.123 0.123v767.775l-0.123 0.122H64.102l-0.122-0.122V128.123l0.122-0.123h895.775zM960 64H64C28.795 64 0 92.795 0 128v768c0 35.205 28.795 64 64 64h896c35.205 0 64-28.795 64-64V128c0-35.205-28.795-64-64-64zM832 288.01c0 53.023-42.988 96.01-96.01 96.01s-96.01-42.987-96.01-96.01S682.967 192 735.99 192 832 234.988 832 288.01zM896 832H128V704l224.01-384 256 320h64l224.01-192z\'></path></svg>',
        'menuKeys': [
          'insertImage',
          'uploadImage'
        ]
      },
      'undo',
      'redo',
    ]
  };

  @Input()
  editorConfig!: IEditorConfig;

  @Input()
  toolbarConfig!: IToolbarConfig;

  @Input()
  language: 'en' | 'zh-cn' = 'zh-cn';

  _value: string = '';
  propagateChange = (_: any) => { };
  onTouched = () => { };

  ngOnDestroy(): void {
    if (this.editor) {
      this.editor.destroy();
      this.editor = undefined as any;
    }
  }

  writeValue(value: any): void {
    this._value = value || '';
    if (this.editor) {
      this.editor.setHtml(value);
    }
  }

  registerOnChange(fn: any): void {
    this.propagateChange = fn;
  }

  registerOnTouched(fn: any): void {
    this.onTouched = fn;
  }

  ngAfterViewInit(): void {
    i18nChangeLanguage(this.language);
    this.editor = createEditor({
      selector: this.editorContainer.nativeElement,
      config: {
        ...this.defaultEditorConfig,
        ...(this.editorConfig ?? {})
      }
    });
    if (this.editor.getConfig().readOnly !== true) {
      this.toolbar = createToolbar({
        editor: this.editor,
        selector: this.editorToolbar.nativeElement,
        config: {
          ...this.defaultToolbarConfig,
          ...(this.toolbarConfig ?? {})
        }
      });
    }
  }
}
