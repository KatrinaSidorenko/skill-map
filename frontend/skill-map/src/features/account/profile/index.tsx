'use client';

import React, { useEffect, useState } from 'react';
import {
  Box,
  VStack,
  HStack,
  Text,
  Input,
  Button,
  Avatar,
  Separator,
  Heading,
  Card,
  Field,
  Stack,
} from '@chakra-ui/react';
import { useForm } from 'react-hook-form';
import { useAppSelector } from '@/store/hooks';
import { selectUser } from '../store';
import {
  useGetProfileQuery,
  useUpdateProfileMutation,
  useResetPasswordMutation,
  useVerifyTokenMutation,
  useSetNewPasswordMutation,
} from '../api';
import { toaster } from '@/components/ui/toaster';
import { retrieveErrorData } from '@/store/helpers';
import useLocalization from '@/i18n/useLocalization';
import { VerifyTokenForm } from '../fogot-password/verifyTokenForm';
import { NewPasswordForm } from '../fogot-password/newPasswordForm';

interface ProfileFormValues {
  username: string;
  email: string;
  avatarUrl: string;
}

type ResetStep = 'idle' | 'verify' | 'newPassword' | 'done';

export default function ProfilePage() {
  const { getProfileTranslations, getAuthTranslations } = useLocalization();
  const user = useAppSelector(selectUser);

  const { data: profile, isLoading: isLoadingProfile } = useGetProfileQuery();
  const [updateProfile, { isLoading: isUpdating }] = useUpdateProfileMutation();
  const [resetPassword, { isLoading: isResetting }] = useResetPasswordMutation();
  const [verifyToken, { isLoading: isVerifying }] = useVerifyTokenMutation();
  const [setNewPassword, { isLoading: isSettingPassword }] = useSetNewPasswordMutation();

  const [avatarPreview, setAvatarPreview] = useState<string>('');
  const [resetStep, setResetStep] = useState<ResetStep>('idle');
  const [resetToken, setResetToken] = useState<string>('');

  const {
    register,
    handleSubmit,
    reset,
    watch,
    formState: { errors, isDirty },
  } = useForm<ProfileFormValues>({
    defaultValues: { username: '', email: '', avatarUrl: '' },
  });

  const watchedAvatarUrl = watch('avatarUrl');

  useEffect(() => {
    if (profile) {
      reset({
        username: profile.username ?? '',
        email: profile.email ?? '',
        avatarUrl: profile.imageUrl ?? '',
      });
      setAvatarPreview(profile.imageUrl ?? '');
    }
  }, [profile, reset]);

  useEffect(() => {
    setAvatarPreview(watchedAvatarUrl);
  }, [watchedAvatarUrl]);

  const onSubmit = async (data: ProfileFormValues) => {
    const payload: UpdateProfileRequest = {};
    if (data.username !== (profile?.username ?? '')) payload.username = data.username || undefined;
    if (data.email !== (profile?.email ?? '')) payload.email = data.email || undefined;
    if (data.avatarUrl !== (profile?.imageUrl ?? '')) payload.imageUrl = data.avatarUrl || undefined;

    if (Object.keys(payload).length === 0) return;

    try {
      await updateProfile(payload).unwrap();
      toaster.create({
        title: getProfileTranslations('updateSuccess'),
        type: 'success',
        closable: true,
      });
    } catch (error) {
      const errorData = retrieveErrorData(error);
      toaster.create({
        title: getProfileTranslations('updateFailed'),
        description: errorData?.message ?? '',
        type: 'error',
        closable: true,
      });
    }
  };

  const handleSendResetLink = async () => {
    if (!profile?.email) return;
    try {
      await resetPassword({ email: profile.email }).unwrap();
      setResetStep('verify');
      toaster.create({
        title: getProfileTranslations('resetEmailSent'),
        type: 'success',
        closable: true,
      });
    } catch {
      toaster.create({
        title: getProfileTranslations('resetFailed'),
        type: 'error',
        closable: true,
      });
    }
  };

  const handleVerifyToken = async (data: { token: string }) => {
    try {
      await verifyToken({ token: data.token }).unwrap();
      setResetToken(data.token);
      setResetStep('newPassword');
    } catch (error) {
      const errorData = retrieveErrorData(error);
      toaster.create({
        title: getAuthTranslations('verifyTokenFailed'),
        description: errorData?.message ?? '',
        type: 'error',
        closable: true,
      });
    }
  };

  const handleSetNewPassword = async (data: { newPassword: string; confirmPassword: string }) => {
    try {
      await setNewPassword({ token: resetToken, password: data.newPassword }).unwrap();
      setResetStep('done');
      toaster.create({
        title: getAuthTranslations('passwordResetSuccess'),
        type: 'success',
        closable: true,
      });
    } catch (error) {
      const errorData = retrieveErrorData(error);
      toaster.create({
        title: getAuthTranslations('resetPasswordFailed'),
        description: errorData?.message ?? '',
        type: 'error',
        closable: true,
      });
    }
  };

  return (
    <Box maxW="2xl" mx="auto" py={8} px={4}>
      <Heading size="xl" mb={8} color="text.heading">
        {getProfileTranslations('title')}
      </Heading>

      {/* Profile Info Card */}
      <Card.Root mb={6} bg="bg.card" borderColor="border.subtle" borderWidth="1px">
        <Card.Header pb={2}>
          <Heading size="md" color="text.heading">
            {getProfileTranslations('profileInfo')}
          </Heading>
          <Text fontSize="sm" color="text.muted" mt={1}>
            {getProfileTranslations('profileInfoDesc')}
          </Text>
        </Card.Header>

        <Card.Body>
          <form onSubmit={handleSubmit(onSubmit)}>
            <VStack gap={6} align="stretch">
              {/* Avatar preview */}
              <HStack gap={6} align="center">
                <Avatar.Root size="2xl" shape="rounded">
                  <Avatar.Fallback name={user?.username ?? 'U'} />
                  <Avatar.Image src={avatarPreview || 'https://avatar.iran.liara.run/public'} />
                </Avatar.Root>
                <Stack flex={1} gap={1}>
                  <Text fontWeight="semibold" color="text.primary">{user?.username}</Text>
                  <Text fontSize="sm" color="text.muted">{user?.email}</Text>
                </Stack>
              </HStack>

              <Separator />

              <Field.Root invalid={!!errors.avatarUrl}>
                <Field.Label fontSize="sm" fontWeight="medium" color="text.primary">
                  {getProfileTranslations('avatarUrl')}
                </Field.Label>
                <Input placeholder={getProfileTranslations('enterAvatarUrl')} size="md" {...register('avatarUrl')} />
              </Field.Root>

              <Field.Root invalid={!!errors.username}>
                <Field.Label fontSize="sm" fontWeight="medium" color="text.primary">
                  {getProfileTranslations('username')}
                </Field.Label>
                <Input placeholder={getProfileTranslations('enterUsername')} size="md" {...register('username', { required: true })} />
              </Field.Root>

              <Field.Root invalid={!!errors.email}>
                <Field.Label fontSize="sm" fontWeight="medium" color="text.primary">
                  {getProfileTranslations('email')}
                </Field.Label>
                <Input placeholder={getProfileTranslations('enterEmail')} type="email" size="md" {...register('email', { required: true })} />
              </Field.Root>

              <Button
                type="submit"
                colorPalette="brand"
                size="sm"
                loading={isUpdating}
                disabled={!isDirty || isUpdating || isLoadingProfile}
                alignSelf="flex-start"
              >
                {getProfileTranslations('saveChanges')}
              </Button>
            </VStack>
          </form>
        </Card.Body>
      </Card.Root>

      {/* Security Card */}
      <Card.Root bg="bg.card" borderColor="border.subtle" borderWidth="1px">
        <Card.Header pb={2}>
          <Heading size="md" color="text.heading">
            {getProfileTranslations('security')}
          </Heading>
          <Text fontSize="sm" color="text.muted" mt={1}>
            {getProfileTranslations('securityDesc')}
          </Text>
        </Card.Header>

        <Card.Body>
          {resetStep === 'idle' && (
            <VStack gap={4} align="stretch">
              <Text fontSize="sm" color="text.primary">
                {getProfileTranslations('resetPasswordDesc')}
              </Text>
              <Button
                colorPalette="red"
                variant="outline"
                size="sm"
                onClick={handleSendResetLink}
                loading={isResetting}
                alignSelf="flex-start"
              >
                {getProfileTranslations('sendResetLink')}
              </Button>
            </VStack>
          )}

          {resetStep === 'verify' && (
            <VerifyTokenForm
              onSubmit={handleVerifyToken}
              isLoading={isVerifying}
              getAuthTranslations={getAuthTranslations}
            />
          )}

          {resetStep === 'newPassword' && (
            <NewPasswordForm
              onSubmit={handleSetNewPassword}
              isLoading={isSettingPassword}
              getAuthTranslations={getAuthTranslations}
            />
          )}

          {resetStep === 'done' && (
            <VStack gap={3} align="stretch">
              <Text fontSize="sm" color="green.500" fontWeight="medium">
                {getAuthTranslations('passwordResetSuccess')}
              </Text>
              <Button
                variant="ghost"
                size="sm"
                alignSelf="flex-start"
                onClick={() => setResetStep('idle')}
              >
                ← Back
              </Button>
            </VStack>
          )}
        </Card.Body>
      </Card.Root>
    </Box>
  );
}
