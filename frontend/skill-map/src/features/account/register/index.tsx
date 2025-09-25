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
    username: z.string().min(3, 'Username must be at least 3 characters long'),
    role: z.string().default('User') as z.ZodType<Role>,
  });

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<RegisterSchema>({
    resolver: zodResolver(accountCreationSchema),
  });

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
          {getAuthTranslations('username')} <Field.RequiredIndicator />
        </Field.Label>
        <Input
          placeholder={getAuthTranslations('eneterUsername')}
          type="text"
          borderColor="brand.100"
          {...register('username')}
        />
        {errors.username && (
          <Field.HelperText fontSize="sm">
            {errors.username.message}
          </Field.HelperText>
        )}
      </Field.Root>

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
          getAuthTranslations('register')
        )}
      </Button>

      <Box mt={6}>
        <Text textAlign="center">
          {getAuthTranslations('haveAccount')}{' '}
          <Link color="brand.300" href="/login" fontWeight="medium">
            {getAuthTranslations('loginHere')}
          </Link>
        </Text>
      </Box>
    </VStack>
  );
}
