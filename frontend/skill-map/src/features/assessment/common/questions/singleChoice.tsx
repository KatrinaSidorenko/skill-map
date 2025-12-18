'use client';

import { VStack, Text, RadioGroup, HStack } from '@chakra-ui/react';
import { useAppDispatch } from '@/store/hooks';
import { setAnswerForQuestion } from '../../store';

export function SingleChoiceOption({
  id,
  text,
  isDisabled,
  isCorrect,
  isSelected, // Assuming this is passed to know what user picked
  isDefault,
}: {
  id: string;
  text: string;
  isDisabled?: boolean;
  isCorrect?: boolean;
  isSelected?: boolean;
  isDefault?: boolean;
}) {
  // 1. Determine "Review Mode" styles (When test is finished/disabled)
  const getReviewStyles = () => {
    if (!isDisabled) return {}; // Not in review mode
    if (isCorrect) {
      return {
        bg: 'green.50',
        borderColor: 'green.500',
        color: 'green.900',
        borderWidth: '1px',
        opacity: 1,
        _checked: {
          bg: 'green.50',
          borderColor: 'green.500',
          color: 'green.900',
        }, // Override default checked style
      };
    }

    // CASE B: User Selected this, but it is WRONG (Red)
    if (isSelected && !isCorrect) {
      return {
        bg: 'red.50',
        borderColor: 'red.500',
        color: 'red.900',
        borderWidth: '1px',
        opacity: 1,
        _checked: { bg: 'red.50', borderColor: 'red.500', color: 'red.900' },
      };
    }

    // CASE C: Just a wrong option that wasn't picked (Dimmed)
    return {
      opacity: 0.5,
      borderWidth: '0px',
    };
  };

  // 2. Standard Interactive Styles (While taking the test)
  const interactiveStyles = {
    bg: 'white',
    borderWidth: '0px',
    cursor: isDisabled ? 'default' : 'pointer',
    transition: 'all 0.2s',
    _hover: isDisabled ? {} : { bg: 'blue.50' },
    _checked: {
      bg: 'blue.50',
      color: 'blue.700',
      borderWidth: '1px',
      borderColor: 'blue.500',
    },
  };

  // 3. Merge Styles (Review styles override Interactive styles)
  const finalStyles = {
    ...interactiveStyles,
    ...getReviewStyles(),
  };

  return (
    <RadioGroup.Item value={id} p={2} borderRadius="md" {...finalStyles}>
      <RadioGroup.ItemHiddenInput />
      <RadioGroup.ItemIndicator />
      <RadioGroup.ItemText>{text}</RadioGroup.ItemText>
    </RadioGroup.Item>
  );
}

export function SingleChoiceGroup({
  value,
  onChange,
  children,
  isDisabled = false,
}: {
  value?: string;
  onChange?: (value: string | null) => void;
  children: React.ReactNode;
  isDisabled?: boolean;
}) {
  return (
    <RadioGroup.Root
      value={value}
      onValueChange={(e) => onChange?.(e.value)}
      disabled={isDisabled}
    >
      <VStack gap={2} align="flex-start">
        {children}
      </VStack>
    </RadioGroup.Root>
  );
}

interface SingleChoiceQuestionProps {
  questionId: string;
  questionText: string;
  answers: { id: string; text: string }[];
}

export function SingleChoiceQuestion(props: SingleChoiceQuestionProps) {
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
    <VStack align="flex-start" gap={4}>
      <Text fontSize="lg" fontWeight="semibold">
        {questionText}
      </Text>

      <SingleChoiceGroup onChange={onAnswerSelect}>
        {answers.map((a) => (
          <SingleChoiceOption
            key={a.id}
            id={a.id}
            text={a.text}
            isDefault={true}
          />
        ))}
      </SingleChoiceGroup>
    </VStack>
  );
}

export function SingleChoiceResult({
  questionText,
  answers,
  achievedPoints,
  totalPossiblePoints,
}: {
  questionText: string;
  answers: SingleChoiceAnswerDetail[];
  achievedPoints?: number;
  totalPossiblePoints?: number;
}) {
  const total =
    typeof totalPossiblePoints === 'number' ? totalPossiblePoints : 1;
  const achieved =
    typeof achievedPoints === 'number'
      ? achievedPoints
      : answers.some((a) => a.isSelected && a.isCorrect)
        ? 1
        : 0;

  const passed = achieved >= total;
  const statusColor = passed ? 'green.600' : 'red.600';

  const selectedId =
    answers.find((a) => a.isSelected)?.answerId ??
    answers.find((a) => a.isCorrect)?.answerId;

  return (
    <VStack align="stretch" gap={4}>
      <HStack justify="space-between" align="center">
        <Text fontSize="lg" fontWeight="semibold">
          {questionText}
        </Text>

        <HStack gap={3}>
          <Text color={statusColor} fontSize="sm">
            {achieved} / {total} pts
          </Text>
        </HStack>
      </HStack>

      <SingleChoiceGroup value={selectedId} isDisabled>
        {answers.map((a) => (
          <SingleChoiceOption
            key={a.answerId}
            id={a.answerId}
            text={a.text}
            isCorrect={a.isCorrect}
            isSelected={a.isSelected}
            isDisabled
          />
        ))}
      </SingleChoiceGroup>
    </VStack>
  );
}
