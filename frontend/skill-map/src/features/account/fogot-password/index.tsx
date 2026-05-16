'use client';
import useLocalization from '@/i18n/useLocalization';
import { VStack, Text, Link, Heading } from '@chakra-ui/react';
import {
  useResetPasswordMutation,
  useSetNewPasswordMutation,
  useVerifyTokenMutation,
} from '../api';
import { useState } from 'react';
import { VerifyTokenForm } from './verifyTokenForm';
import { NewPasswordForm } from './newPasswordForm';
import { RequestResetForm } from './resetForm';
import { toaster } from '@/components/ui/toaster';
import { retrieveErrorData } from '@/store/helpers';

type Step = 'request' | 'verify' | 'newPassword' | 'done';

export default function ForgotPasswordComponent() {
  const { getAuthTranslations } = useLocalization();

  const [step, setStep] = useState<Step>('request');
  const [token, setToken] = useState<string>('');

  const [triggerReset, { isLoading: isResetting }] = useResetPasswordMutation();
  const [triggerVerifyToken, { isLoading: isVerifying }] =
    useVerifyTokenMutation();
  const [triggerSetNewPassword, { isLoading: isSettingNewPassword }] =
    useSetNewPasswordMutation();

  // Step 1: request reset
  const onEmailSubmit = async (data: { email: string }) => {
    try {
      await triggerReset({ email: data.email }).unwrap();
      setStep('verify');
    } catch (error) {
      const errorData = retrieveErrorData(error);
      let description = '';
      if (errorData) {
        description = getAuthTranslations(errorData.code);
      }
      toaster.create({
        title: getAuthTranslations('resetPasswordFailed'),
        type: 'error',
        description: description,
        closable: true,
      });
    }
  };

  // Step 2: verify token
  const onVerifySubmit = async (data: { token: string }) => {
    setToken(data.token);
    try {
      await triggerVerifyToken({ token: data.token }).unwrap();
      setStep('newPassword');
    } catch (error) {
      const errorData = retrieveErrorData(error);
      let description = '';
      if (errorData) {
        description = getAuthTranslations(errorData.code);
      }
      toaster.create({
        title: getAuthTranslations('verifyTokenFailed'),
        type: 'error',
        description: description,
        closable: true,
      });
    }
  };

  // Step 3: set new password
  const onPasswordSubmit = async (data: {
    newPassword: string;
    confirmPassword: string;
  }) => {
    try {
      await triggerSetNewPassword({
        token,
        password: data.newPassword,
      }).unwrap();
      setStep('done');
    } catch (error) {
      const errorData = retrieveErrorData(error);
      let description = '';
      if (errorData) {
        description = getAuthTranslations(errorData.code);
      }
      toaster.create({
        title: getAuthTranslations('setNewPasswordFailed'),
        type: 'error',
        description: description,
        closable: true,
      });
    }
  };

  return (
    <VStack w="full" gap={5}>
      <VStack gap={1} align="flex-start" w="full" mb={2}>
        <Heading size="lg" color="text.heading" fontWeight="800">
          {getAuthTranslations('resetPassword')}
        </Heading>
        <Text fontSize="sm" color="text.muted">
          {step === 'request' && getAuthTranslations('resetRequestSubtitle')}
          {step === 'verify' && getAuthTranslations('resetVerifySubtitle')}
          {step === 'newPassword' && getAuthTranslations('resetNewPasswordSubtitle')}
        </Text>
      </VStack>
      {step === 'request' && (
        <RequestResetForm
          onSubmit={onEmailSubmit}
          isLoading={isResetting}
          getAuthTranslations={getAuthTranslations}
        />
      )}
      {step === 'verify' && (
        <VerifyTokenForm
          onSubmit={onVerifySubmit}
          isLoading={isVerifying}
          getAuthTranslations={getAuthTranslations}
        />
      )}
      {step === 'newPassword' && (
        <NewPasswordForm
          onSubmit={onPasswordSubmit}
          isLoading={isSettingNewPassword}
          getAuthTranslations={getAuthTranslations}
        />
      )}
      {step === 'done' && (
        <>
          <Text fontWeight="bold" textAlign="center" color="text.heading">
            {getAuthTranslations('passwordResetSuccess')}
          </Text>
          <Link color="brand.500" href="/login" fontWeight="semibold" fontSize="sm">
            {getAuthTranslations('loginHere')}
          </Link>
        </>
      )}
    </VStack>
  );
}
