'use client';

import { VStack, Text, RadioGroup } from '@chakra-ui/react';
import { useAppDispatch } from '@/store/hooks';
import { setAnswerForQuestion } from '../../store';

interface SingleChoiceQuestionProps {
  questionId: string;
  questionText: string;
  answers: { id: string; text: string }[];
}

export default function SingleChoiceQuestion(props: SingleChoiceQuestionProps) {
  const dispatch = useAppDispatch();
  const { questionId, questionText, answers } = props;

  const onAnswerSelect = (answerId: string | null) => {
    if (!answerId) return;
    dispatch(
      setAnswerForQuestion({
        answer: {
          questionId,
          type: 'single_choice' as TestQuestionType,
          selectedAnswerId: answerId,
        },
      }),
    );
  };

  return (
    <VStack align="stretch" gap={4}>
      <Text fontSize="lg" fontWeight="semibold">
        {questionText}
      </Text>
      <RadioGroup.Root onValueChange={(e) => onAnswerSelect(e.value)}>
        <VStack gap={2} align="flex-start">
          {answers.map((item) => (
            <RadioGroup.Item
              key={item.id}
              value={item.id}
              cursor="pointer"
              p={2}
              borderRadius="md"
              borderWidth="0px"
              _hover={{ bg: 'gray.50' }}
              _checked={{
                bg: 'blue.50',
                color: 'blue.700',
              }}
            >
              <RadioGroup.ItemHiddenInput />
              <RadioGroup.ItemIndicator />
              <RadioGroup.ItemText>{item.text}</RadioGroup.ItemText>
            </RadioGroup.Item>
          ))}
        </VStack>
      </RadioGroup.Root>
    </VStack>
  );
}
