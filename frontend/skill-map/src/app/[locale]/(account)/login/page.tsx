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

export default function LoginPage() {
  const [visible, setVisible] = useState(false);
  return (
    <VStack gap={4} w="full" h="full" justify="center" px={12}>
      <Field.Root required>
        <Field.Label>
          Email <Field.RequiredIndicator />
        </Field.Label>
        <Input placeholder="Enter your email" type="email" />
      </Field.Root>

      <Field.Root required>
        <Field.Label>
          Password <Field.RequiredIndicator />
        </Field.Label>
        <PasswordInput
          defaultValue="password"
          visible={visible}
          onVisibleChange={setVisible}
        />
      </Field.Root>

      <Button width="full" variant="outline">
        Login
      </Button>

      <Text textAlign="center">
        <Link color="text.accent" href="/forgot-password" fontWeight="medium">
          Forgot your password?
        </Link>
      </Text>

      <Box mt={4}>
        <Text textAlign="center">
          Don't have an account?{' '}
          <Link color="text.accent" href="/signup" fontWeight="medium">
            Sign up here
          </Link>
        </Text>
      </Box>
    </VStack>
  );
}
