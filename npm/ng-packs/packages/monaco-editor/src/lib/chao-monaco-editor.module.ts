import { FormsModule } from '@angular/forms';
import { NgModule } from '@angular/core';
import { ChaoMonacoEditorComponent } from './chao-monaco-editor.component';
import { ChaoCoreModule } from '@nankingcigar/ng.core';

@NgModule({
  declarations: [ChaoMonacoEditorComponent],
  imports: [FormsModule, ChaoCoreModule],
  exports: [ChaoMonacoEditorComponent],
})
export class ChaoMonacoEditorModule { }
