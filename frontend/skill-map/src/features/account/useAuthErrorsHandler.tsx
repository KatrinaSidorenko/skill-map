import { toaster } from '@/components/ui/toaster';
import useLocalization from '@/i18n/useLocalization';
import { retrieveErrorData } from '@/store/helpers';

type AccountActions =
  | 'login'
  | 'register'
  | 'resetPassword'
  | 'setNewPassword'
  | 'verifyToken';

type Failed =
  | 'loginFailed'
  | 'registerFailed'
  | 'resetPasswordFailed'
  | 'setNewPasswordFailed'
  | 'verifyTokenFailed';
  
const actionsMap: Record<AccountActions, Failed> = {
  login: 'loginFailed',
  register: 'registerFailed',
  resetPassword: 'resetPasswordFailed',
  setNewPassword: 'setNewPasswordFailed',
  verifyToken: 'verifyTokenFailed',
};

export function useAuthErrorsHandler(action: AccountActions) {
  const { getAuthTranslations } = useLocalization();

  const triggerWrapper = async (trigger: Promise<unknown>) => {
    try {
      await trigger;
    } catch (error) {
      const errorData = retrieveErrorData(error);
      let description = '';
      if (errorData) {
        description = getAuthTranslations(errorData.code);
      }
      toaster.create({
        title: getAuthTranslations(actionsMap[action]),
        type: 'error',
        description: description,
        closable: true,
      });
    }
  };

  return { triggerWrapper };
}
