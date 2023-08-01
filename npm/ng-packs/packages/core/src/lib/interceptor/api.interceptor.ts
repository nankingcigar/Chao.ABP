/*
 * @Author: Chao Yang
 * @Date: 2020-12-19 06:58:04
 * @LastEditor: Chao Yang
 * @LastEditTime: 2022-08-09 14:19:56
 */
import { Injectable } from '@angular/core';
import {
  HttpEvent,
  HttpInterceptor,
  HttpHandler,
  HttpRequest,
  HttpResponse,
  HttpErrorResponse,
} from '@angular/common/http';
import { Observable, of, Subject } from 'rxjs';
import { catchError, delay, map } from 'rxjs/operators';
import { InterceptorService } from './interceptor.service';

@Injectable({
  providedIn: 'root',
})
export class ApiInterceptor implements HttpInterceptor {
  cacheRequest: {
    [key: string]: boolean
  } = {};
  cacheSubject: {
    [key: string]: Subject<HttpEvent<any>>
  } = {};

  constructor(private interceptorService: InterceptorService) { }

  intercept(
    req: HttpRequest<any>,
    next: HttpHandler
  ): Observable<HttpEvent<any>> {
    const key = req.method + 'δ' + req.urlWithParams + 'δ' + JSON.stringify(req.body ?? '');
    if (this.cacheRequest[key]) {
      if (!this.cacheSubject[key]) {
        this.cacheSubject[key] = new Subject<HttpEvent<any>>();
      }
      return this.cacheSubject[key].asObservable();
    }
    this.cacheRequest[key] = true;
    req = this.interceptorService.handleBeforeRequest(req);
    return next
      .handle(
        req
      )
      .pipe(
        map((response) => {
          if (response instanceof HttpResponse) {
            try {
              response = this.interceptorService.handleSuccessResponse(response);
              this.success(key, response);
            } catch (error) {
              this.error(key, error);
              throw error;
            }
          }
          return response;
        }),
        catchError((error) => {
          if (error instanceof HttpErrorResponse) {
            try {
              error = this.interceptorService.handleErrorResponse(error);
            } catch (error) {
              error = error;
            }
            this.error(key, error);
          }
          throw error;
        })
      );
  }

  success(key: string, response: HttpEvent<any>): void {
    if (this.cacheSubject[key]) {
      const subscription = of(response).pipe(delay(0)).subscribe((response) => {
        this.cacheSubject[key].next(response);
        this.cacheSubject[key].complete();
        this.cacheSubject[key].unsubscribe();
        delete this.cacheSubject[key];
        subscription.unsubscribe();
      });
    }
    delete this.cacheRequest[key];
  }

  error(key: string, error: any): void {
    if (this.cacheSubject[key]) {
      const subscription = of(error).pipe(delay(0)).subscribe((error) => {
        this.cacheSubject[key].error(error);
        this.cacheSubject[key].complete();
        this.cacheSubject[key].unsubscribe();
        delete this.cacheSubject[key];
        subscription.unsubscribe();
      });
    }
    delete this.cacheRequest[key];
  }
}
