/*
 * @Author: Chao Yang
 * @Date: 2020-12-11 17:23:38
 * @LastEditor: Chao Yang
 * @LastEditTime: 2022-11-26 16:32:46
 */
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { ModuleWithProviders, NgModule } from '@angular/core';
import {
  AuthConfig,
  OAuthModule,
  OAuthStorage
} from 'angular-oauth2-oidc';
import { ApiInterceptor } from './interceptor/api.interceptor';
import { InterceptorService } from './interceptor/interceptor.service';
import { AuthenticationConfig } from './model/authentication/index';
import { RemoveHostTagDirective } from './directive/index';

@NgModule({
  declarations: [
    RemoveHostTagDirective
  ],
  imports: [
    OAuthModule.forRoot({
      resourceServer: {
        sendAccessToken: true,
      },
    })
  ],
  exports: [
    RemoveHostTagDirective
  ],
})
export class ChaoCoreModule {
  static forRoot(options: {
    authenticationConfig: AuthenticationConfig;
    interceptor: typeof InterceptorService;
  }): ModuleWithProviders<ChaoCoreModule> {
    return {
      ngModule: ChaoCoreModule,
      providers: [
        {
          provide: AuthenticationConfig,
          useValue: options.authenticationConfig,
        },
        {
          provide: AuthConfig,
          useValue: options.authenticationConfig.tokenConfig,
        },
        {
          provide: HTTP_INTERCEPTORS,
          useExisting: ApiInterceptor,
          multi: true,
        },
        {
          provide: InterceptorService,
          useClass: options.interceptor ?? InterceptorService,
        },
        { provide: OAuthStorage, useValue: localStorage }
      ],
    };
  }
}
