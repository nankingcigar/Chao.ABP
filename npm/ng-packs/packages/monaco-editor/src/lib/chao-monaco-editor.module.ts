import { FormsModule } from '@angular/forms';
import { NgModule } from '@angular/core';
import { ChaoMonacoEditorComponent } from './chao-monaco-editor.component';

@NgModule({
  declarations: [ChaoMonacoEditorComponent],
  imports: [FormsModule],
  exports: [ChaoMonacoEditorComponent],
})
export class ChaoMonacoEditorModule {}
