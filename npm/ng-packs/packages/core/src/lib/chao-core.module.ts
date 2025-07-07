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

@NgModule({
  declarations: [
  ],
  imports: [
    OAuthModule.forRoot({
      resourceServer: {
        sendAccessToken: true,
      },
    })
  ],
  exports: [
  ],
})
export class ChaoCoreModule {
  static forRoot(options: {
    authenticationConfig: AuthenticationConfig;
    interceptor: typeof InterceptorService;
    cacheUploadRequest?: boolean;
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
        { provide: OAuthStorage, useValue: localStorage },
        {
          provide: 'CACHE_UPLOAD_REQUEST',
          useValue: options.cacheUploadRequest ?? false,
        }
      ]
    };
  }
}
