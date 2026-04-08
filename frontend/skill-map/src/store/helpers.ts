import { MOCK_IMAGE_URL } from '@/store/mock';

export const retrieveErrorData = (error: unknown): ResponseInfo | null => {
  if (typeof error === 'object' && error !== null && 'data' in error) {
    const responseInfo = error.data as ResponseInfo;
    return responseInfo;
  }
  return null;
};

export const normalizeImageUrl = (imageUrl: string) => {
  if (imageUrl) {
    return imageUrl;
  }

  return MOCK_IMAGE_URL;
};
