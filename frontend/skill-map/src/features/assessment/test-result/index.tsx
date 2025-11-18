'use client';

import { Box, Heading, VStack, Button, HStack, Text } from '@chakra-ui/react';
import { useAppSelector } from '@/store/hooks';
import { useRouter } from 'next/navigation';
import { selectCheckedQuestionResults } from '../store';
import { QuestionResultFactory } from '../common/questions/factory';

export default function TestResults() {
  const router = useRouter();
  const checkedQuestionResults = useAppSelector(selectCheckedQuestionResults);
  // todo: if any checked results missing, redirect to home page
  const onBackHome = () => {
    router.replace('/home');
  };

  const totalAchieved = checkedQuestionResults?.totalAchievedPoints ?? 0;
  const totalPossible = checkedQuestionResults?.totalPossiblePoints ?? 0;

  return (
    <Box width="80%" p={6} borderRadius="md" bg="white" padding={20}>
      <Box mb={6}>
        <Heading size="lg">Test Results</Heading>

        <HStack mt={3} gap={4} align="center">
          <Text color={'gray.600'} fontSize="sm">
            {totalAchieved} / {totalPossible} pts
          </Text>
        </HStack>
      </Box>

      <VStack gap={6} align="stretch">
        {Object.values(checkedQuestionResults?.questionResults ?? {})?.map(
          (q) => (
            <QuestionResultFactory key={q.questionId} question={q} />
          ),
        )}
      </VStack>

      <Box mt={6} display="flex" justifyContent="flex-end">
        <Button size="sm" variant="solid" onClick={onBackHome}>
          Back to Home
        </Button>
      </Box>
    </Box>
  );
}
