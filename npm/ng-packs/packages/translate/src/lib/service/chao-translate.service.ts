import { Injectable } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { map, Observable } from 'rxjs';

@Injectable()
export class ChaoTranslateService {
  constructor(private translateService: TranslateService) { }

  get(key: string | Array<string>, interpolateParams?: Object): Observable<string | string[] | any> {
    return this.translateService.get(key, interpolateParams).pipe(
      map((translation: (string | any)) => {
        if (typeof (key) === 'object') {
          const translations = (key as string[]).map(k => (translation as { [key: string]: string })[k]);
          return translations;
        }
        return translation;
      })
    );
  }

  instant(key: string | Array<string>, interpolateParams?: Object): string | string[] | any {
    return this.translateService.instant(key, interpolateParams);
  }
}
