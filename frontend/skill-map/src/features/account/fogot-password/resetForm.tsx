import { Button, Field, Input, VStack } from '@chakra-ui/react';
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
      <Field.Root required invalid={!!errors.email}>
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

      <Button
        type="submit"
        colorPalette="brand"
        size="sm"
        w="full"
        loading={isLoading}
      >
        {getAuthTranslations('sendResetLink')}
      </Button>
    </VStack>
  );
}
