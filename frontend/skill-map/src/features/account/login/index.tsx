'use client';
import {
  Field,
  Input,
  Button,
  Link,
  VStack,
  Text,
  Heading,
  Separator,
} from '@chakra-ui/react';
import { PasswordInput } from '@/components/ui/password-input';
import { useState } from 'react';
import useLocalization from '@/i18n/useLocalization';
import { useRouter } from 'next/navigation';
import { useForm } from 'react-hook-form';
import { z } from 'zod';
import { zodResolver } from '@hookform/resolvers/zod';
import { useLoginMutation } from '../api';
import { useAuth } from '../useAuthContext';
import { useAuthErrorsHandler } from '../useAuthErrorsHandler';

type LoginSchema = {
  email: string;
  password: string;
};

export default function LoginComponent() {
  const [visible, setVisible] = useState(false);
  const { getAuthTranslations } = useLocalization();
  const router = useRouter();
  const { login: setToken } = useAuth();
  const { triggerWrapper } = useAuthErrorsHandler('login');

  const loginSchema = z.object({
    email: z.string(),
    password: z.string().min(6, getAuthTranslations('passwordMinLength')),
  });

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<LoginSchema>({
    resolver: zodResolver(loginSchema),
  });

  const [login, { isLoading }] = useLoginMutation();
  const onSubmit = async (data: LoginSchema) => {
    const loginAction = async () => {
      const res = await login(data).unwrap();
      setToken(res.token);
      router.push('/home');
    };

    await triggerWrapper(loginAction());
  };

  return (
    <VStack gap={5} w="full" as="form" onSubmit={handleSubmit(onSubmit)}>
      <VStack gap={1} align="flex-start" w="full" mb={2}>
        <Heading size="lg" color="text.heading" fontWeight="800">
          {getAuthTranslations('welcomeBack')}
        </Heading>
        <Text fontSize="sm" color="text.muted">
          {getAuthTranslations('loginSubtitle')}
        </Text>
      </VStack>

      <Field.Root required invalid={!!errors.email} w="full">
        <Field.Label fontSize="sm" fontWeight="medium">
          {getAuthTranslations('email')} <Field.RequiredIndicator />
        </Field.Label>
        <Input
          placeholder={getAuthTranslations('enterEmail')}
          type="email"
          size="md"
          {...register('email')}
        />
        {errors.email && (
          <Field.ErrorText fontSize="xs">{errors.email.message}</Field.ErrorText>
        )}
      </Field.Root>

      <Field.Root required invalid={!!errors.password} w="full">
        <Field.Label fontSize="sm" fontWeight="medium">
          {getAuthTranslations('password')} <Field.RequiredIndicator />
        </Field.Label>
        <PasswordInput
          visible={visible}
          onVisibleChange={setVisible}
          {...register('password')}
        />
        {errors.password && (
          <Field.ErrorText fontSize="xs">{errors.password.message}</Field.ErrorText>
        )}
      </Field.Root>

      <Link
        color="brand.500"
        href="/forgot-password"
        fontWeight="medium"
        fontSize="sm"
        alignSelf="flex-end"
        mt={-2}
      >
        {getAuthTranslations('forgotPassword')}
      </Link>

      <Button
        type="submit"
        colorPalette="brand"
        size="sm"
        w="full"
        loading={isLoading}
        mt={2}
      >
        {getAuthTranslations('login')}
      </Button>

      <Separator />

      <Text fontSize="sm" color="text.muted" textAlign="center">
        {getAuthTranslations('noAccount')}{' '}
        <Link color="brand.500" href="/register" fontWeight="semibold">
          {getAuthTranslations('signUpHere')}
        </Link>
      </Text>
    </VStack>
  );
}
