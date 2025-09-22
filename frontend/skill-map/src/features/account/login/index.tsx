'use client';
import {
  Field,
  Input,
  Button,
  Link,
  VStack,
  Text,
  Box,
  Spinner,
} from '@chakra-ui/react';
import { PasswordInput } from '@/components/ui/password-input';
import { useState } from 'react';
import useLocalization from '@/i18n/useLocalization';
import { useRouter } from 'next/navigation';
import { useForm } from 'react-hook-form';
import { z } from 'zod';
import { zodResolver } from '@hookform/resolvers/zod';
import { useLoginMutation } from '../api';
import { toaster } from '@/components/ui/toaster';
import { getResponseInfo as retrieveErrorData } from '@/store/helpers';
import { useAuth } from '../useAuthContext';

type LoginSchema = {
  email: string;
  password: string;
};

export default function LoginComponent() {
  const [visible, setVisible] = useState(false);
  const { getAuthTranslations } = useLocalization();
  const router = useRouter();
  const { login: setToken } = useAuth();

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
    try {
      const res = await login(data).unwrap();
      setToken(res.token);
      router.push('/home');
    } catch (error) {
      const errorData = retrieveErrorData(error);
      let description = '';
      if (errorData) {
        description = getAuthTranslations(errorData.code);
      }

      toaster.create({
        title: getAuthTranslations('loginFailed'),
        type: 'error',
        description: description,
        closable: true,
      });
    }
  };

  return (
    <VStack
      gap={4}
      w="full"
      h="full"
      justify="center"
      px={12}
      as="form"
      onSubmit={handleSubmit(onSubmit)}
    >
      <Field.Root required>
        <Field.Label>
          {getAuthTranslations('email')} <Field.RequiredIndicator />
        </Field.Label>
        <Input
          placeholder={getAuthTranslations('enterEmail')}
          type="email"
          borderColor="brand.100"
          {...register('email')}
        />
        {errors.email && (
          <Field.HelperText fontSize="sm">
            {errors.email.message}
          </Field.HelperText>
        )}
      </Field.Root>

      <Field.Root required>
        <Field.Label>
          {getAuthTranslations('password')} <Field.RequiredIndicator />
        </Field.Label>
        <PasswordInput
          defaultValue="password"
          visible={visible}
          onVisibleChange={setVisible}
          borderColor="brand.100"
          {...register('password')}
        />
        {errors.password && (
          <Field.HelperText fontSize="sm">
            {errors.password.message}
          </Field.HelperText>
        )}
      </Field.Root>

      <Button
        type="submit"
        width="full"
        variant="outline"
        mt={8}
        disabled={isLoading}
      >
        {isLoading ? (
          <Spinner color="blue.500" animationDuration="0.8s" size="sm" />
        ) : (
          getAuthTranslations('login')
        )}
      </Button>

      <Text textAlign="center">
        <Link color="text.accent" href="/forgot-password" fontWeight="medium">
          {getAuthTranslations('forgotPassword')}
        </Link>
      </Text>

      <Box mt={6}>
        <Text textAlign="center">
          {getAuthTranslations('noAccount')}{' '}
          <Link color="text.accent" href="/register" fontWeight="medium">
            {getAuthTranslations('signUpHere')}
          </Link>
        </Text>
      </Box>
    </VStack>
  );
}
