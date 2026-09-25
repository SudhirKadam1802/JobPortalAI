import { HttpInterceptorFn } from '@angular/common/http';

export const authInterceptor: HttpInterceptorFn = (req, next) => {

  const token = localStorage.getItem('careerai_token');

  // Do not attach an old JWT to authentication requests.
  const isAuthRequest =
    req.url.includes('/api/Auth/login') ||
    req.url.includes('/api/Auth/register');

  if (token && !isAuthRequest) {

    const authReq = req.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`
      }
    });

    return next(authReq);
  }

  return next(req);
};