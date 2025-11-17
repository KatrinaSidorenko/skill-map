'use client';
import { useGenerateRoadmapTestMutation } from '@/features/assessment/api';
import TestForm from '@/features/assessment/test-form';
import { Box, Flex } from '@chakra-ui/react';
import { useEffect } from 'react';

export default function AssessmentPanelPage() {
  //   const [checkAnswers, { isLoading }] = useCheckRoadmapTestAnswersMutation();
  const [generateTest, { isLoading }] = useGenerateRoadmapTestMutation();

  useEffect(() => {
    const fetchTest = async () => {
      try {
        const result = await generateTest({
          testId: 'bb01bde3fdc14e17ad3e3aa9813d6ef2',
          config: {
            numberOfQuestions: 1,
            timeLimitInMinutes: 30,
            difficultyLevel: 'easy',
          },
        }).unwrap();
        console.log('Generated Test:', result);
      } catch (error) {
        console.error('Error generating test:', error);
      }
    };

    fetchTest();
  }, []);

  if (isLoading) {
    return <div>Loading...</div>;
  }

  return (
    <Box
      display="flex"
      justifyContent="center"
      bg="bg.section"
      alignItems="center"
      padding={10}
    >
      <TestForm />
    </Box>
  );
}
