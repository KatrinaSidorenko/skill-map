import { Button, Field, Input, VStack } from '@chakra-ui/react';
import { z } from 'zod';
import { zodResolver } from '@hookform/resolvers/zod';
import { useForm } from 'react-hook-form';

const schema = z.object({
  token: z.string().min(6, 'Token must be at least 6 characters'),
});

type FormValues = z.infer<typeof schema>;

export function VerifyTokenForm({
  onSubmit,
  isLoading,
  getAuthTranslations,
}: {
  onSubmit: (data: FormValues) => void | Promise<void>;
  isLoading: boolean;
  getAuthTranslations: (key: keyof ILocalization['auth']) => string;
}) {
  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<FormValues>({
    resolver: zodResolver(schema),
  });

  return (
    <VStack as="form" onSubmit={handleSubmit(onSubmit)} gap={4} w="full">
      <Field.Root required invalid={!!errors.token}>
        <Field.Label fontSize="sm" fontWeight="medium">
          {getAuthTranslations('token')} <Field.RequiredIndicator />
        </Field.Label>
        <Input
          placeholder={getAuthTranslations('enterToken')}
          type="text"
          size="md"
          {...register('token')}
        />
        {errors.token && (
          <Field.ErrorText fontSize="xs">{errors.token.message}</Field.ErrorText>
        )}
      </Field.Root>

      <Button
        type="submit"
        colorPalette="brand"
        size="sm"
        w="full"
        loading={isLoading}
      >
        {getAuthTranslations('verifyToken')}
      </Button>
    </VStack>
  );
}
