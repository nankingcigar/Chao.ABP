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
} from '@angular/common/http';
import { Observable } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { InterceptorService } from './interceptor.service';

@Injectable({
  providedIn: 'root',
})
export class ApiInterceptor implements HttpInterceptor {
  constructor(private interceptorService: InterceptorService) { }

  intercept(
    req: HttpRequest<any>,
    next: HttpHandler
  ): Observable<HttpEvent<any>> {
    req = this.interceptorService.handleBeforeRequest(req);
    return next
      .handle(
        req
      )
      .pipe(
        map((response) => {
          return this.interceptorService.handleSuccessResponse(response);
        }),
        catchError((errorResponse) => {
          return this.interceptorService.handleErrorResponse(errorResponse);
        })
      );
  }
}
