'use client';
import { Button, Field, Input, Spinner, VStack } from '@chakra-ui/react';
import { z } from 'zod';
import { zodResolver } from '@hookform/resolvers/zod';
import { useForm } from 'react-hook-form';
import { PasswordInput } from '@/components/ui/password-input';
import { useState } from 'react';

const schema = z
  .object({
    newPassword: z.string().min(6, 'Password must be at least 6 characters'),
    confirmPassword: z.string(),
  })
  .refine((data) => data.newPassword === data.confirmPassword, {
    message: 'Passwords must match',
  });
type FormValues = z.infer<typeof schema>;

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
      <Field.Root required>
        <Field.Label>
          {getAuthTranslations('newPassword')} <Field.RequiredIndicator />
        </Field.Label>
        <PasswordInput
          defaultValue="password"
          visible={isPasswordVisible}
          onVisibleChange={setPasswordVisible}
          borderColor="brand.100"
          {...register('newPassword')}
        />
        {errors.newPassword && (
          <Field.HelperText color="red.500" fontSize="sm">
            {errors.newPassword.message}
          </Field.HelperText>
        )}
      </Field.Root>

      <Field.Root required>
        <Field.Label>
          {getAuthTranslations('confirmPassword')} <Field.RequiredIndicator />
        </Field.Label>
        <PasswordInput
          defaultValue="password"
          visible={isConfirmPasswordVisible}
          onVisibleChange={setConfirmPasswordVisible}
          borderColor="brand.100"
          {...register('confirmPassword')}
        />
        {errors.confirmPassword && (
          <Field.HelperText color="red.500" fontSize="sm">
            {errors.confirmPassword.message}
          </Field.HelperText>
        )}
      </Field.Root>

      <Button type="submit" width="full" variant="outline" disabled={isLoading}>
        {isLoading ? (
          <Spinner color="blue.500" animationDuration="0.8s" size="sm" />
        ) : (
          getAuthTranslations('setNewPassword')
        )}
      </Button>
    </VStack>
  );
}
