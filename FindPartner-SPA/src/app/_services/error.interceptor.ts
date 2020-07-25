import { Injectable } from '@angular/core';
import {
  HttpInterceptor,
  HttpErrorResponse,
  HTTP_INTERCEPTORS,
  HttpRequest,
  HttpHandler,
  HttpEvent,
} from '@angular/common/http';
import { catchError } from 'rxjs/operators';
import { throwError, Observable } from 'rxjs';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {
  intercept(
    req: HttpRequest<any>,
    next: HttpHandler
  ): Observable<HttpEvent<any>> {
    return next.handle(req).pipe(
        catchError(error => {
            if (error.status === 401)
            {
                return throwError(error.statusText);
            }
            if (error instanceof HttpErrorResponse)
                {
                    // Application Error handle ...
                    const applicationError = error.headers.get('Application-Error');
                    if (applicationError)
                    {
                        return throwError(applicationError);
                    }

                    // Model State Error and server error Handle ...
                    const serverError = error.error;
                    let modalStateErrors = '';
                    if (serverError.errors && typeof serverError.errors === 'object')
                    {
                        for (const key in serverError.error)
                        {
                            if (serverError.errors[key])
                            {
                                modalStateErrors += serverError.error[key] + '\n';

                            }
                        }
                    }
                    return throwError(modalStateErrors || serverError || 'Server Error- Need Investigation.');
                }
        })
    );
  }
}

export const ErrorInterceptorProvider = {
    provide: HTTP_INTERCEPTORS,
    useClass: ErrorInterceptor,
    multi: true
}