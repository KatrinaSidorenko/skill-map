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
import { useRegisterMutation } from '../api';
import { useAuthErrorsHandler } from '../useAuthErrorsHandler';

type RegisterSchema = {
  username: string;
  email: string;
  password: string;
  role: unknown;
};

export default function RegisterComponent() {
  const [visible, setVisible] = useState(false);
  const { getAuthTranslations } = useLocalization();
  const { triggerWrapper } = useAuthErrorsHandler('register');
  const router = useRouter();

  const accountCreationSchema = z.object({
    email: z.string(),
    password: z.string().min(6, getAuthTranslations('passwordMinLength')),
    username: z.string().min(3, getAuthTranslations('usernameMinLength3')),
    role: z.string().default('User') as z.ZodType<Role>,
  });

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<RegisterSchema>({ resolver: zodResolver(accountCreationSchema) });

  const [userRegister, { isLoading }] = useRegisterMutation();

  const onSubmit = async (data: RegisterSchema) => {
    const registerAction = async () => {
      await userRegister({
        email: data.email,
        password: data.password,
        username: data.username,
        role: data.role as Role,
      }).unwrap();
      router.push('/login');
    };
    await triggerWrapper(registerAction());
  };

  return (
    <VStack gap={5} w="full" as="form" onSubmit={handleSubmit(onSubmit)}>
      <VStack gap={1} align="flex-start" w="full" mb={2}>
        <Heading size="lg" color="text.heading" fontWeight="800">
          {getAuthTranslations('createAccount')}
        </Heading>
        <Text fontSize="sm" color="text.muted">
          {getAuthTranslations('registerSubtitle')}
        </Text>
      </VStack>

      <Field.Root required invalid={!!errors.username} w="full">
        <Field.Label fontSize="sm" fontWeight="medium">
          {getAuthTranslations('username')} <Field.RequiredIndicator />
        </Field.Label>
        <Input
          placeholder={getAuthTranslations('eneterUsername')}
          type="text"
          size="md"
          {...register('username')}
        />
        {errors.username && (
          <Field.ErrorText fontSize="xs">{errors.username.message}</Field.ErrorText>
        )}
      </Field.Root>

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

      <Button
        type="submit"
        colorPalette="brand"
        size="sm"
        w="full"
        loading={isLoading}
        mt={2}
      >
        {getAuthTranslations('register')}
      </Button>

      <Separator />

      <Text fontSize="sm" color="text.muted" textAlign="center">
        {getAuthTranslations('haveAccount')}{' '}
        <Link color="brand.500" href="/login" fontWeight="semibold">
          {getAuthTranslations('loginHere')}
        </Link>
      </Text>
    </VStack>
  );
}
