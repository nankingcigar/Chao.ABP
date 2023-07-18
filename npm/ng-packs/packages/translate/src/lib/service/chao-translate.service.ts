/*
 * @Author: Chao Yang
 * @Date: 2022-09-07 23:52:41
 * @LastEditor: Chao Yang
 * @LastEditTime: 2022-09-08 00:08:34
 */
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
}
