import { ModuleWithProviders, NgModule, Provider } from '@angular/core';
import { TranslateModule } from '@ngx-translate/core';
import { provideTranslateHttpLoader, TranslateHttpLoaderConfig } from '@ngx-translate/http-loader';
import { ChaoTranslateService } from './service/chao-translate.service';

export function provideChaoTranslate(folder?: string): Provider[] {
  return provideTranslateHttpLoader({
    prefix: `assets/i18n/${folder ?? ''}`,
    suffix: '.json',
  } as Partial<TranslateHttpLoaderConfig>);
}

@NgModule({
  imports: [
    TranslateModule.forRoot({
      fallbackLang: 'zh-cn',
      isolate: false
    }),
  ],
  exports: [TranslateModule],
  providers: [ChaoTranslateService],
})
export class ChaoTranslateModule {
  static forRoot(folder?: string): ModuleWithProviders<ChaoTranslateModule> {
    return {
      ngModule: ChaoTranslateModule,
      providers: [
        ...provideChaoTranslate(folder),
        ChaoTranslateService,
      ],
    };
  }
  
  static forChild(): ModuleWithProviders<TranslateModule> {
    return {
      ngModule: TranslateModule,
    };
  }
}
