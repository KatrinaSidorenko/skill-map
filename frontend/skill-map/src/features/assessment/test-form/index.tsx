'use client';

import { VStack, Button, Heading, Box } from '@chakra-ui/react';
import { useAppSelector } from '@/store/hooks';
import { selectTestQuestions } from '../store';
import { QuestionsFactory } from '../common/questions/factory';
import { useCheckRoadmapTestAnswersMutation } from '../api';
import { useRouter } from 'next/navigation';

export default function TestForm() {
  const router = useRouter();
  const testQuestions = useAppSelector(selectTestQuestions);
  const [checkAnswers, { isLoading }] = useCheckRoadmapTestAnswersMutation();

  const onSubmit = async () => {
    // Handle submission
  };

  const onCancel = () => {
    router.replace('/home');
  };

  return (
    <Box width="80%" p={6} borderRadius="md" bg="white" padding={20}>
      <Box mb={6}>
        <Heading size="lg">Assessment Test</Heading>
      </Box>

      <VStack gap={6} align="stretch">
        {testQuestions?.map((q) => (
          <QuestionsFactory key={q.id} {...q} />
        ))}
      </VStack>

      <Box mt={6} display="flex" justifyContent="flex-end">
        <Button
          size="md"
          mr={4}
          variant="ghost"
          onClick={onCancel}
          bg="red.200"
        >
          Cancel
        </Button>
        <Button size="sm" onClick={onSubmit} loading={isLoading}>
          Submit Test
        </Button>
      </Box>
    </Box>
  );
}
