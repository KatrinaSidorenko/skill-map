type ErrorCode = 'SE6' | 'UIE1';

interface ResponseInfo {
  message: string;
  code: ErrorCode;
}
