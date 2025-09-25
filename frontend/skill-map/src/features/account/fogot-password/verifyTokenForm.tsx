import { Button, Field, Input, Spinner, VStack } from '@chakra-ui/react';
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
      <Field.Root required>
        <Field.Label>
          {getAuthTranslations('token')} <Field.RequiredIndicator />
        </Field.Label>
        <Input
          placeholder={getAuthTranslations('enterToken')}
          type="text"
          {...register('token')}
        />
        {errors.token && (
          <Field.HelperText color="red.500" fontSize="sm">
            {errors.token.message}
          </Field.HelperText>
        )}
      </Field.Root>

      <Button type="submit" width="full" variant="outline" disabled={isLoading}>
        {isLoading ? (
          <Spinner color="blue.500" animationDuration="0.8s" size="sm" />
        ) : (
          getAuthTranslations('verifyToken')
        )}
      </Button>
    </VStack>
  );
}
