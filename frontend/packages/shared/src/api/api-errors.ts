export const ApiErrors = {
  badRequest: 'bad-request',
  unauthorized: 'unauthorized',
  forbidden: 'forbidden',
  notFound: 'not-found',
  conflict: 'conflict',
  requestError: 'request-error',
  serverError: 'server-error',
} as const;

export type ApiError = (typeof ApiErrors)[keyof typeof ApiErrors];

const errorsByStatus: Record<number, ApiError> = {
  400: ApiErrors.badRequest,
  401: ApiErrors.unauthorized,
  403: ApiErrors.forbidden,
  404: ApiErrors.notFound,
  409: ApiErrors.conflict,
};

// Callers need to tell one failure from another — "the user is not registered" reads
// as a 404 and nothing else. statusText cannot carry that: it is English prose and
// servers are free to omit it.
export const apiErrorForStatus = (status: number): ApiError =>
  errorsByStatus[status] ?? (status >= 500 ? ApiErrors.serverError : ApiErrors.requestError);
