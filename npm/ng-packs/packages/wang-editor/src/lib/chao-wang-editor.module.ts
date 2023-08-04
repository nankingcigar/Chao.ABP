import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ChaoWangEditorComponent } from './chao-wang-editor.component';
import { Boot } from '@wangeditor/editor';
import formulaModule from '@wangeditor/plugin-formula';
Boot.registerModule((formulaModule as any).default);

@NgModule({
  declarations: [
    ChaoWangEditorComponent
  ],
  imports: [
    FormsModule
  ],
  exports: [
    ChaoWangEditorComponent
  ]
})
export class ChaoWangEditorModule {
}
