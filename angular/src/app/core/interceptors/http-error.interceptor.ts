import {
  HttpErrorResponse,
  HttpEvent,
  HttpHandler,
  HttpInterceptor,
  HttpRequest,
} from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Router } from "@angular/router";
import { ToastMessageType } from "@shared/AppEnums";
import { AppRoutes } from "@shared/AppRoutes";
import { MessageService } from "primeng/api";
import { Observable, throwError } from "rxjs";
import { catchError } from "rxjs/operators";

@Injectable()
export class HttpErrorInterceptor implements HttpInterceptor {
  constructor(private router: Router, private _message: MessageService) {}

  intercept(
    request: HttpRequest<any>,
    next: HttpHandler
  ): Observable<HttpEvent<any>> {
    return next
      .handle(request)
      .pipe(catchError((error) => this.handleAuthError(error)));
  }

  private handleAuthError(err: HttpErrorResponse): Observable<any> {
    if (err.status === 401) {
      this.router.navigateByUrl(AppRoutes.ACCOUNT.LOGIN);
      abp.notify.error(
        "Your session is expired, please login again",
        "Authentication Failed"
      );
      return;
    }

    if (err.error instanceof HttpErrorResponse || err.error) {
      const errorObj = err.error?.error;
      errorObj?.message &&
        this._message.add({
          severity: ToastMessageType.ERROR,
          summary: err.statusText || "Exist an error!",
          detail: errorObj.message,
        });
    }

    return throwError(err);
  }
}
