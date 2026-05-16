'use client';
import { Button, Field, VStack } from '@chakra-ui/react';
import { z } from 'zod';
import { zodResolver } from '@hookform/resolvers/zod';
import { useForm } from 'react-hook-form';
import { PasswordInput } from '@/components/ui/password-input';
import { useState } from 'react';

type FormValues = { newPassword: string; confirmPassword: string };

export function NewPasswordForm({
  onSubmit,
  isLoading,
  getAuthTranslations,
}: {
  onSubmit: (data: FormValues) => Promise<void>;
  isLoading: boolean;
  getAuthTranslations: (key: keyof ILocalization['auth']) => string;
}) {
  const [isPasswordVisible, setPasswordVisible] = useState(false);
  const [isConfirmPasswordVisible, setConfirmPasswordVisible] = useState(false);

  const schema = z
    .object({
      newPassword: z.string().min(6, 'Password must be at least 6 characters'),
      confirmPassword: z.string(),
    })
    .refine((data) => data.newPassword === data.confirmPassword, {
      message: getAuthTranslations('passwordsMustMatch'),
      path: ['confirmPassword'],
    });

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<FormValues>({
    resolver: zodResolver(schema),
  });

  return (
    <VStack as="form" onSubmit={handleSubmit(onSubmit)} gap={4} w="full">
      <Field.Root required invalid={!!errors.newPassword}>
        <Field.Label fontSize="sm" fontWeight="medium">
          {getAuthTranslations('newPassword')} <Field.RequiredIndicator />
        </Field.Label>
        <PasswordInput
          visible={isPasswordVisible}
          onVisibleChange={setPasswordVisible}
          {...register('newPassword')}
        />
        {errors.newPassword && (
          <Field.ErrorText fontSize="xs">{errors.newPassword.message}</Field.ErrorText>
        )}
      </Field.Root>

      <Field.Root required invalid={!!errors.confirmPassword}>
        <Field.Label fontSize="sm" fontWeight="medium">
          {getAuthTranslations('confirmPassword')} <Field.RequiredIndicator />
        </Field.Label>
        <PasswordInput
          visible={isConfirmPasswordVisible}
          onVisibleChange={setConfirmPasswordVisible}
          {...register('confirmPassword')}
        />
        {errors.confirmPassword && (
          <Field.ErrorText fontSize="xs">{errors.confirmPassword.message}</Field.ErrorText>
        )}
      </Field.Root>

      <Button
        type="submit"
        colorPalette="brand"
        size="sm"
        w="full"
        loading={isLoading}
      >
        {getAuthTranslations('setNewPassword')}
      </Button>
    </VStack>
  );
}
