'use client';

import { VStack, Text, RadioGroup, HStack } from '@chakra-ui/react';
import { useAppDispatch } from '@/store/hooks';
import { setAnswerForQuestion } from '../../store';

export function SingleChoiceOption({
  id,
  text,
  isDisabled,
  isCorrect,
}: {
  id: string;
  text: string;
  isDisabled?: boolean;
  isCorrect?: boolean;
}) {
  const baseProps = {
    value: id,
    p: 2,
    borderRadius: 'md',
    borderWidth: '0px',
    transition: 'background-color 0.12s, color 0.12s',
  };

  const cursor = isDisabled ? 'default' : 'pointer';
  const opacity = isDisabled ? 0.5 : 1;

  const hoverStyle = isDisabled
    ? {}
    : {
        bg: isCorrect ? 'green.50' : 'gray.50',
      };

  const checkedStyle = isDisabled
    ? {
        bg: isCorrect ? 'green.50' : 'gray.100',
        color: isCorrect ? 'green.700' : 'red.500',
      }
    : isCorrect
      ? { bg: 'green.50', color: 'green.700' }
      : { bg: 'red.50', color: 'red.700' };

  const borderStyle = isCorrect
    ? { borderWidth: '1px', borderColor: 'green.100' }
    : {};

  return (
    <RadioGroup.Item
      {...baseProps}
      cursor={cursor}
      opacity={opacity}
      _hover={hoverStyle}
      _checked={checkedStyle}
      disabled={isDisabled}
      {...borderStyle}
    >
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
          <SingleChoiceOption key={a.id} id={a.id} text={a.text} />
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
            isDisabled
          />
        ))}
      </SingleChoiceGroup>
    </VStack>
  );
}
