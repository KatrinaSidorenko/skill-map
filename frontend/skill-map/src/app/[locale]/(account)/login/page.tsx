'use client';
import {
  Field,
  Input,
  Button,
  Link,
  VStack,
  Text,
  Box,
} from '@chakra-ui/react';
import { PasswordInput } from '@/components/ui/password-input';
import { useState } from 'react';
import useLocalization from '@/i18n/useLocalization';

export default function LoginPage() {
  const [visible, setVisible] = useState(false);
  const { getAuthTranslations } = useLocalization();
  return (
    <VStack gap={4} w="full" h="full" justify="center" px={12}>
      <Field.Root required>
        <Field.Label>
          {getAuthTranslations('email')} <Field.RequiredIndicator />
        </Field.Label>
        <Input
          placeholder={getAuthTranslations('enterEmail')}
          type="email"
          shadow="sm"
        />
      </Field.Root>

      <Field.Root required>
        <Field.Label>
          {getAuthTranslations('password')} <Field.RequiredIndicator />
        </Field.Label>
        <PasswordInput
          defaultValue="password"
          visible={visible}
          onVisibleChange={setVisible}
          shadow="sm"
        />
      </Field.Root>

      <Button width="full" variant="outline" mt={8}>
        {getAuthTranslations('login')}
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
