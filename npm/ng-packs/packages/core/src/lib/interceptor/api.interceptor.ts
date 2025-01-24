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
import { catchError, delay, finalize, map } from 'rxjs/operators';
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
        }),
        finalize(() => {
          this.cancel(key);
        })
      );
  }

  success(key: string, response: HttpEvent<any>): void {
    if (this.cacheSubject[key]) {
      const cacheSubject = this.cacheSubject[key];
      const subscription = of(response).pipe(delay(0)).subscribe((response) => {
        cacheSubject.next(response);
        cacheSubject.complete();
        cacheSubject.unsubscribe();
        subscription.unsubscribe();
      });
      delete this.cacheSubject[key];
    }
    delete this.cacheRequest[key];
  }

  error(key: string, error: any): void {
    if (this.cacheSubject[key]) {
      const cacheSubject = this.cacheSubject[key];
      const subscription = of(error).pipe(delay(0)).subscribe((error) => {
        cacheSubject.error(error);
        cacheSubject.complete();
        cacheSubject.unsubscribe();
        subscription.unsubscribe();
      });
      delete this.cacheSubject[key];
    }
    delete this.cacheRequest[key];
  }

  cancel(key: string): void {
    if (this.cacheSubject[key]) {
      const cacheSubject = this.cacheSubject[key];
      const subscription = of(undefined).pipe(delay(0)).subscribe((n) => {
        cacheSubject.complete();
        cacheSubject.unsubscribe();
        subscription.unsubscribe();
      });
      delete this.cacheSubject[key];
    }
    delete this.cacheRequest[key];
  }
}
