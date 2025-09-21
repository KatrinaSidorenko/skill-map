export const getResponseInfo = (error: unknown): ResponseInfo | null => {
  if (typeof error === 'object' && error !== null && 'data' in error) {
    const responseInfo = error.data as ResponseInfo;
    return responseInfo;
  }
  return null;
};
