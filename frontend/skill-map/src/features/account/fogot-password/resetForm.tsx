import { Button, Field, Input, Spinner, VStack } from '@chakra-ui/react';
import { z } from 'zod';
import { zodResolver } from '@hookform/resolvers/zod';
import { useForm } from 'react-hook-form';

const schema = z.object({
  email: z.string(),
});

type FormValues = z.infer<typeof schema>;

export function RequestResetForm({
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
    <VStack
      as="form"
      onSubmit={handleSubmit(onSubmit)}
      gap={4}
      w="full"
      justify="center"
    >
      <Field.Root required>
        <Field.Label>
          {getAuthTranslations('email')} <Field.RequiredIndicator />
        </Field.Label>
        <Input
          placeholder={getAuthTranslations('enterEmail')}
          type="email"
          {...register('email')}
        />
        {errors.email && (
          <Field.HelperText color="red.500" fontSize="sm">
            {errors.email.message}
          </Field.HelperText>
        )}
      </Field.Root>

      <Button type="submit" width="full" variant="outline" disabled={isLoading}>
        {isLoading ? (
          <Spinner color="blue.500" animationDuration="0.8s" size="sm" />
        ) : (
          getAuthTranslations('sendResetLink')
        )}
      </Button>
    </VStack>
  );
}
