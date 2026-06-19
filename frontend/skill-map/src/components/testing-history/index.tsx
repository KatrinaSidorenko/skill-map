'use client';

import React, { useMemo } from 'react';
import {
  Accordion,
  Badge,
  Box,
  Button,
  Flex,
  HStack,
  Separator,
  Span,
  Spinner,
  Stack,
  Text,
  VStack,
} from '@chakra-ui/react';
import useLocalization from '@/i18n/useLocalization';

type Props = {
  data?: TestingHistoryDto | null;
  isLoading?: boolean;
  isInitialTestGenerating?: boolean;
  isIntermediateTestGenerating?: boolean;

  onGenerateInitialTest?: () => void;
  onGenerateIntermediateTest?: () => void;
  onOpenAttempt?: (args: { testId: string; attemptId: string }) => void;
  onContinueAttempt?: (args: { testId: string; attemptId: string }) => void;
  onStartNewAttempt?: (args: { testId: string }) => void;

  title?: string;
};

function safeDate(iso: string | null | undefined): Date | null {
  if (!iso) return null;
  const d = new Date(iso);
  return Number.isNaN(d.getTime()) ? null : d;
}

function formatDateTime(iso: string | null | undefined) {
  const d = safeDate(iso);
  if (!d) return '—';
  return new Intl.DateTimeFormat(undefined, {
    year: 'numeric',
    month: 'short',
    day: '2-digit',
    hour: '2-digit',
    minute: '2-digit',
  }).format(d);
}

function clamp(n: number, min: number, max: number) {
  return Math.max(min, Math.min(max, n));
}

function attemptStatus(a: TestAttemptDto) {
  if (a.completedAt && typeof a.score === 'number') return 'completed';
  if (!a.completedAt) return 'in_progress';
  return 'unknown';
}

function calcDurationMinutes(startedAt: string, completedAt: string | null) {
  const s = safeDate(startedAt);
  const e = safeDate(completedAt);
  if (!s || !e) return null;
  const mins = Math.round((e.getTime() - s.getTime()) / 60000);
  return mins >= 0 ? mins : null;
}

function bestScore(attempts: TestAttemptDto[]) {
  const scores = attempts
    .map((a) => (typeof a.score === 'number' ? a.score : null))
    .filter((x): x is number => x !== null);
  if (!scores.length) return null;
  return Math.max(...scores);
}

function lastAttempt(attempts: TestAttemptDto[]) {
  if (!attempts.length) return null;
  return [...attempts].sort(
    (a, b) => new Date(b.startedAt).getTime() - new Date(a.startedAt).getTime(),
  )[0];
}

function scoreTone(score: number, max: number) {
  const pct = max > 0 ? (score / max) * 100 : 0;
  if (pct >= 80) return { colorPalette: 'green' as const };
  if (pct >= 50) return { colorPalette: 'yellow' as const };
  return { colorPalette: 'red' as const };
}

function mapAccordionDefaults(
  items: Array<{ testId: string; attempts: TestAttemptDto[] }>,
) {
  // Open tests that have an in-progress attempt by default
  return items
    .filter((t) => {
      const last = lastAttempt(t.attempts);
      return last && attemptStatus(last) === 'in_progress';
    })
    .map((t) => t.testId);
}

function isInitialTestingExist(data: TestingHistoryDto | null | undefined) {
  //return true;
  if (!data) return false;
  return data.items.some((item) => item.type === 'initial');
}

export default function TestingHistory({
  data,
  isLoading,
  isInitialTestGenerating,
  isIntermediateTestGenerating,
  onOpenAttempt,
  onContinueAttempt,
  onStartNewAttempt,
  onGenerateInitialTest,
  onGenerateIntermediateTest,
  title,
}: Props) {
  const { getTestingHistoryTranslations } = useLocalization();
  const items = data?.items ?? [];
  const hasInitialTesting = isInitialTestingExist(data);

  const normalized = useMemo(() => {
    return items.map((t) => {
      const best = bestScore(t.attempts);
      const last = lastAttempt(t.attempts);

      const completedCount = t.attempts.filter(
        (a) => attemptStatus(a) === 'completed',
      ).length;
      const inProgressCount = t.attempts.filter(
        (a) => attemptStatus(a) === 'in_progress',
      ).length;

      return { ...t, best, last, completedCount, inProgressCount };
    });
  }, [items]);

  const defaultOpen = useMemo(
    () => mapAccordionDefaults(normalized),
    [normalized],
  );

  return (
    <Box>
      <Flex align="center" justify="space-between" mb={4} gap={3} flexWrap="wrap">
        <HStack gap={3}>
          <Box w="10px" h="10px" borderRadius="full" bg="bg.primaryAccent" />
          <Text fontSize="lg" fontWeight="800" color="text.heading">
            {title ?? getTestingHistoryTranslations('title')}
          </Text>
        </HStack>

        <HStack gap={3} flexWrap="wrap">
          {onGenerateIntermediateTest &&
            data?.isIntermediateAssessmentAvailable && (
              <Button
                onClick={onGenerateIntermediateTest}
                loading={isIntermediateTestGenerating}
                disabled={isIntermediateTestGenerating}
                size="sm"
                variant="ghost"
              >
                {getTestingHistoryTranslations('takeIntermediateTest')}
              </Button>
            )}
          {isLoading ? (
            <Badge>{getTestingHistoryTranslations('loading')}</Badge>
          ) : (
            <Badge>
              {normalized.length}{' '}
              {normalized.length === 1
                ? getTestingHistoryTranslations('attempt')
                : getTestingHistoryTranslations('attempts')}
            </Badge>
          )}
        </HStack>
      </Flex>

      {isLoading ? (
        <Box bg="bg.section" borderRadius="2xl" borderWidth="1px" p={5}>
          <VStack align="stretch" gap={3}>
            <Spinner />
          </VStack>
        </Box>
      ) : normalized.length === 0 ? (
        <Box bg="bg.section" borderRadius="2xl" borderWidth="1px" p={6}>
          <VStack align="start" gap={2}>
            <Text fontWeight="800" color="text.heading">
              {getTestingHistoryTranslations('noTestsYet')}
            </Text>
            <Text color="text.primary" opacity={0.8}>
              {getTestingHistoryTranslations('noTestsDescription')}
            </Text>
            {!hasInitialTesting &&
              !data?.isIntermediateAssessmentAvailable &&
              onGenerateInitialTest && (
                <Button
                  mt={2}
                  onClick={() => onGenerateInitialTest()}
                  loading={isInitialTestGenerating}
                  size="sm"
                >
                  {getTestingHistoryTranslations('generateInitialTest')}
                </Button>
              )}
          </VStack>
        </Box>
      ) : (
        <>
          <Accordion.Root collapsible defaultValue={defaultOpen}>
            {normalized.map((t) => {
              const bestPct =
                typeof t.best === 'number' && t.maxScore > 0
                  ? (t.best / t.maxScore) * 100
                  : null;

              const headerBest =
                typeof t.best === 'number' ? (
                  <Badge {...scoreTone(t.best, t.maxScore)}>
                    {getTestingHistoryTranslations('best')}: {t.best}/
                    {t.maxScore}
                  </Badge>
                ) : (
                  <Badge>{getTestingHistoryTranslations('best')}: —</Badge>
                );

              return (
                <Accordion.Item
                  key={t.testId}
                  value={t.testId}
                  mb={3}
                  border="none"
                  borderRadius="2xl"
                  overflow="hidden"
                  bg="bg.section"
                  boxShadow="0 6px 20px rgba(0,0,0,0.04)"
                >
                  <Accordion.ItemTrigger
                    px={5}
                    py={4}
                    _hover={{ bg: 'bg.accent' }}
                  >
                    <Flex
                      w="full"
                      align="center"
                      justify="space-between"
                      gap={4}
                      wrap="wrap"
                    >
                      <HStack gap={3} minW={0}>
                        <Box
                          w="36px"
                          h="36px"
                          borderRadius="xl"
                          bg="bg.accent"
                          display="grid"
                          placeItems="center"
                          borderWidth="1px"
                          borderColor="border.default"
                          flexShrink={0}
                        >
                          <Span fontWeight="900">
                            {t.type?.slice(0, 1)?.toUpperCase() ?? 'T'}
                          </Span>
                        </Box>

                        <Box minW={0}>
                          <HStack gap={2} wrap="wrap">
                            <Text fontWeight="800" color="text.heading">
                              {t.type}
                            </Text>
                            {/*<Badge>ID: {t.testId}</Badge>*/}
                          </HStack>

                          <Stack direction="row" gap={2} mt={1} wrap="wrap">
                            <Badge>
                              {getTestingHistoryTranslations('max')}:{' '}
                              {t.maxScore}
                            </Badge>
                            <Badge>
                              {getTestingHistoryTranslations('attempts')}:{' '}
                              {t.attempts.length}
                            </Badge>
                            {t.completedCount > 0 && (
                              <Badge colorPalette="green">
                                {getTestingHistoryTranslations('completed')}:{' '}
                                {t.completedCount}
                              </Badge>
                            )}
                            {t.inProgressCount > 0 && (
                              <Badge colorPalette="yellow">
                                {getTestingHistoryTranslations('inProgress')}:{' '}
                                {t.inProgressCount}
                              </Badge>
                            )}
                          </Stack>
                        </Box>
                      </HStack>

                      <HStack gap={3} flexShrink={0}>
                        {headerBest}
                        <Accordion.ItemIndicator />
                      </HStack>
                    </Flex>
                  </Accordion.ItemTrigger>

                  <Accordion.ItemContent>
                    <Accordion.ItemBody px={5} py={4}>
                      <Stack gap={4}>
                        {/* Best score progress */}
                        <Box>
                          <HStack justify="space-between" mb={2}>
                            <Text
                              fontSize="sm"
                              color="text.primary"
                              fontWeight="700"
                            >
                              {getTestingHistoryTranslations('bestScore')}
                            </Text>
                            <Text
                              fontSize="sm"
                              color="text.primary"
                              opacity={0.8}
                            >
                              {typeof bestPct === 'number'
                                ? `${Math.round(bestPct)}%`
                                : '—'}
                            </Text>
                          </HStack>
                          <Badge>
                            {typeof bestPct === 'number'
                              ? clamp(bestPct, 0, 100)
                              : 0}
                          </Badge>
                        </Box>

                        <Separator />

                        {/* Actions */}
                        <HStack justify="space-between" wrap="wrap" gap={3} align="start">
                          <HStack gap={2} wrap="wrap" flex="1">
                            {t.last &&
                              attemptStatus(t.last) === 'in_progress' && (
                                <Button
                                  size="sm"
                                  onClick={() =>
                                    onContinueAttempt?.({
                                      testId: t.testId,
                                      attemptId: t.last!.resultId,
                                    })
                                  }
                                >
                                  {getTestingHistoryTranslations(
                                    'continueLast',
                                  )}
                                </Button>
                              )}

                            <Button
                              size="sm"
                              variant="outline"
                              onClick={() =>
                                onStartNewAttempt?.({ testId: t.testId })
                              }
                            >
                              {getTestingHistoryTranslations('startNewAttempt')}
                            </Button>
                          </HStack>

                          {t.last ? (
                            <Text
                              fontSize="sm"
                              color="text.primary"
                              opacity={0.85}
                            >
                              {getTestingHistoryTranslations('lastStarted')}:{' '}
                              {formatDateTime(t.last.startedAt)}
                            </Text>
                          ) : (
                            <Text
                              fontSize="sm"
                              color="text.primary"
                              opacity={0.7}
                            >
                              {getTestingHistoryTranslations('noAttemptsYet')}
                            </Text>
                          )}
                        </HStack>

                        {/* Attempts */}
                        <VStack align="stretch" gap={3}>
                          {t.attempts
                            .slice()
                            .sort(
                              (a, b) =>
                                new Date(b.startedAt).getTime() -
                                new Date(a.startedAt).getTime(),
                            )
                            .map((a, idx) => {
                              const status = attemptStatus(a);
                              const duration = calcDurationMinutes(
                                a.startedAt,
                                a.completedAt,
                              );
                              const pct =
                                typeof a.score === 'number' && t.maxScore > 0
                                  ? (a.score / t.maxScore) * 100
                                  : null;

                              return (
                                <Box
                                  key={a.resultId}
                                  borderWidth="1px"
                                  borderColor="border.default"
                                  borderRadius="xl"
                                  bg="bg.page"
                                  p={4}
                                >
                                  <Flex
                                    align="start"
                                    justify="space-between"
                                    gap={4}
                                    wrap="wrap"
                                  >
                                    <Box>
                                      <HStack gap={2} wrap="wrap">
                                        <Text
                                          fontWeight="800"
                                          color="text.heading"
                                        >
                                          {getTestingHistoryTranslations(
                                            'attempt',
                                          )}{' '}
                                          #{t.attempts.length - idx}
                                        </Text>
                                        <Badge>
                                          {getTestingHistoryTranslations(
                                            'result',
                                          )}
                                          : {a.resultId}
                                        </Badge>

                                        {status === 'completed' ? (
                                          <Badge colorPalette="green">
                                            {getTestingHistoryTranslations(
                                              'completed',
                                            )}
                                          </Badge>
                                        ) : status === 'in_progress' ? (
                                          <Badge colorPalette="yellow">
                                            {getTestingHistoryTranslations(
                                              'inProgress',
                                            )}
                                          </Badge>
                                        ) : (
                                          <Badge>
                                            {getTestingHistoryTranslations(
                                              'unknown',
                                            )}
                                          </Badge>
                                        )}
                                      </HStack>

                                      <Stack
                                        direction="row"
                                        mt={2}
                                        gap={3}
                                        wrap="wrap"
                                      >
                                        <Text
                                          fontSize="sm"
                                          color="text.primary"
                                          opacity={0.85}
                                        >
                                          {getTestingHistoryTranslations(
                                            'started',
                                          )}
                                          : {formatDateTime(a.startedAt)}
                                        </Text>
                                        <Text
                                          fontSize="sm"
                                          color="text.primary"
                                          opacity={0.85}
                                        >
                                          {getTestingHistoryTranslations(
                                            'completedLabel',
                                          )}
                                          : {formatDateTime(a.completedAt)}
                                        </Text>
                                        <Text
                                          fontSize="sm"
                                          color="text.primary"
                                          opacity={0.85}
                                        >
                                          {getTestingHistoryTranslations(
                                            'duration',
                                          )}
                                          :{' '}
                                          {duration !== null
                                            ? `${duration} ${getTestingHistoryTranslations('min')}`
                                            : '—'}
                                        </Text>
                                      </Stack>
                                    </Box>

                                     <VStack
                                       align="end"
                                       gap={2}
                                       minW={{ base: 'full', sm: '200px' }}
                                       flex="1 1 200px"
                                     >
                                      <HStack
                                        gap={2}
                                        wrap="wrap"
                                        justify="flex-end"
                                      >
                                        {typeof a.score === 'number' ? (
                                          <Badge
                                            {...scoreTone(a.score, t.maxScore)}
                                          >
                                            {getTestingHistoryTranslations(
                                              'score',
                                            )}
                                            : {a.score}/{t.maxScore}
                                          </Badge>
                                        ) : (
                                          <Badge>
                                            {getTestingHistoryTranslations(
                                              'score',
                                            )}
                                            : —
                                          </Badge>
                                        )}

                                        <Button
                                          size="sm"
                                          variant="outline"
                                          onClick={() =>
                                            onOpenAttempt?.({
                                              testId: t.testId,
                                              attemptId: a.resultId,
                                            })
                                          }
                                        >
                                          {getTestingHistoryTranslations(
                                            'open',
                                          )}
                                        </Button>
                                      </HStack>

                                      <Badge w="full" borderRadius="full">
                                        {typeof pct === 'number'
                                          ? `${Math.round(pct)}%`
                                          : '—'}
                                      </Badge>
                                    </VStack>
                                  </Flex>
                                </Box>
                              );
                            })}
                        </VStack>
                      </Stack>
                    </Accordion.ItemBody>
                  </Accordion.ItemContent>
                </Accordion.Item>
              );
            })}
          </Accordion.Root>
        </>
      )}
    </Box>
  );
}
