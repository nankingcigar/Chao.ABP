import { ModuleWithProviders, NgModule, Provider } from '@angular/core';
import {
  TranslateDirective,
  TranslatePipe,
  provideTranslateService,
} from '@ngx-translate/core';
import { provideTranslateHttpLoader } from '@ngx-translate/http-loader';
import { ChaoTranslateService } from './service/chao-translate.service';

export function provideChaoTranslate(folder?: string): Provider[] {
  return provideTranslateService({
    loader: provideTranslateHttpLoader({
      prefix: `/assets/i18n/${folder ?? ''}`,
      suffix: '.json',
    }),
    fallbackLang: 'zh-cn',
  });
}

@NgModule({
  imports: [TranslatePipe, TranslateDirective],
  exports: [TranslatePipe, TranslateDirective],
  providers: [ChaoTranslateService],
})
export class ChaoTranslateModule {
  static forRoot(folder?: string): ModuleWithProviders<ChaoTranslateModule> {
    return {
      ngModule: ChaoTranslateModule,
      providers: [...provideChaoTranslate(folder), ChaoTranslateService],
    };
  }

  static forChild(): ModuleWithProviders<ChaoTranslateModule> {
    return {
      ngModule: ChaoTranslateModule,
    };
  }
}
