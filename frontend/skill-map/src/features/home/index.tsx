'use client';

import React from 'react';
import {
  Badge,
  Box,
  Button,
  Flex,
  Grid,
  Heading,
  Icon,
  Progress,
  Separator,
  Stack,
  Text,
  VStack,
} from '@chakra-ui/react';
import {
  FiBookmark,
  FiCheckCircle,
  FiTrendingUp,
  FiActivity,
} from 'react-icons/fi';
import { useRouter } from 'next/navigation';
import useLocalization from '@/i18n/useLocalization';
import { useGetDashboardQuery } from '@/features/account/api';
import SpinnerScreen from '@/components/base/spinner';
import ErrorScreen from '@/components/base/error';

interface StatCardProps {
  label: string;
  value: string | number;
  icon: React.ElementType;
}

function StatCard({ label, value, icon }: StatCardProps) {
  return (
    <Flex
      bg="bg.section"
      borderRadius="xl"
      p={5}
      gap={4}
      align="center"
      boxShadow="0 1px 3px rgba(0,0,0,0.06)"
      flex={1}
      minW="140px"
      borderWidth="1px"
      borderColor="border.muted"
    >
      <Flex
        bg="brand.800"
        borderRadius="lg"
        p={3}
        align="center"
        justify="center"
        flexShrink={0}
      >
        <Icon as={icon} boxSize={5} color="brand.200" />
      </Flex>
      <VStack align="start" gap={0}>
        <Text
          fontSize="2xl"
          fontWeight="800"
          color="text.heading"
          lineHeight="1.2"
        >
          {value}
        </Text>
        <Text fontSize="xs" color="text.muted" fontWeight="medium">
          {label}
        </Text>
      </VStack>
    </Flex>
  );
}

// ─── Progress Row ─────────────────────────────────────────────────────────────

interface ProgressRowProps {
  roadmap: InProgressRoadmap;
  progressLabel: string;
  onClick: (workspaceId: string) => void;
}

function ProgressRow({ roadmap, progressLabel, onClick }: ProgressRowProps) {
  const pct = Math.min(100, Math.max(0, Math.round(roadmap.progress * 100)));
  return (
    <Box
      bg="bg.section"
      borderRadius="lg"
      p={4}
      boxShadow="sm"
      cursor="pointer"
      _hover={{ boxShadow: 'md', transform: 'translateY(-1px)' }}
      transition="all 0.15s"
      onClick={() => onClick(roadmap.workspaceId)}
    >
      <Flex justify="space-between" mb={2}>
        <Text fontWeight="semibold" color="text.heading" fontSize="sm">
          {roadmap.title}
        </Text>
        <Text fontSize="sm" color="text.muted" fontWeight="medium">
          {progressLabel}: {pct}%
        </Text>
      </Flex>
      <Progress.Root
        value={pct}
        size="sm"
        colorPalette="teal"
        borderRadius="full"
      >
        <Progress.Track borderRadius="full">
          <Progress.Range />
        </Progress.Track>
      </Progress.Root>
    </Box>
  );
}

// ─── Recent Test Row ──────────────────────────────────────────────────────────

interface TestRowProps {
  test: RecentAssessmentAttempt;
  scoreLabel: string;
  inProgressLabel: string;
}

function TestRow({ test, scoreLabel, inProgressLabel }: TestRowProps) {
  const isCompleted = test.status === 'completed' && test.score !== null;
  const pct = isCompleted
    ? Math.round((test.score! / test.maxScore) * 100)
    : null;

  const scoreColor =
    pct === null ? 'gray' : pct >= 80 ? 'green' : pct >= 50 ? 'yellow' : 'red';

  return (
    <Flex
      bg="bg.section"
      borderRadius="lg"
      p={4}
      boxShadow="sm"
      align="center"
      gap={4}
    >
      <Icon
        as={isCompleted ? FiCheckCircle : FiActivity}
        boxSize={5}
        color={isCompleted ? 'green.500' : 'yellow.500'}
        flexShrink={0}
      />
      <Box flex={1} minW={0}>
        <Text fontWeight="semibold" color="text.heading" fontSize="sm" truncate>
          {test.type}
        </Text>
        <Text fontSize="xs" color="text.muted">
          {test.completedAt
            ? new Date(test.completedAt).toLocaleDateString()
            : '—'}
        </Text>
      </Box>
      {isCompleted ? (
        <VStack gap={0} align="end" flexShrink={0}>
          <Badge colorPalette={scoreColor} variant="subtle" px={2}>
            {scoreLabel}: {test.score}/{test.maxScore}
          </Badge>
          <Text fontSize="xs" color="text.muted">
            {pct}%
          </Text>
        </VStack>
      ) : (
        <Badge colorPalette="orange" variant="subtle">
          {inProgressLabel}
        </Badge>
      )}
    </Flex>
  );
}

export default function HomePage() {
  const router = useRouter();
  const { getHomeTranslations } = useLocalization();
  const t = getHomeTranslations;

  const {
    data: dashboard,
    isLoading,
    isError,
    refetch,
  } = useGetDashboardQuery();

  if (isLoading) return <SpinnerScreen />;
  if (isError || !dashboard) return <ErrorScreen onRetry={refetch} />;

  const { stats, inProgressRoadmaps, recentTests } = dashboard;

  return (
    <Stack gap={8} pb={8} h="80vh">
      <Flex gap={4} wrap="wrap">
        <StatCard
          label={t('savedRoadmaps')}
          value={stats.savedRoadmaps}
          icon={FiBookmark}
        />
        <StatCard
          label={t('testsCompleted')}
          value={stats.testsCompleted}
          icon={FiCheckCircle}
        />
        <StatCard
          label={t('averageScore')}
          value={`${Math.round(stats.averageScore)}${t('percent')}`}
          icon={FiTrendingUp}
        />
        <StatCard
          label={t('activeRoadmaps')}
          value={stats.activeRoadmaps}
          icon={FiActivity}
        />
      </Flex>

      {/* Main grid */}
      <Grid templateColumns={{ base: '1fr', lg: '1fr 1fr' }} gap={6}>
        {/* In-Progress Roadmaps */}
        <Box>
          <Flex justify="space-between" align="center" mb={3}>
            <Heading size="md" color="text.heading">
              {t('inProgressSection')}
            </Heading>
            <Button
              variant="ghost"
              size="xs"
              color="brand.500"
              onClick={() => router.push('/saved')}
            >
              {t('viewAll')}
            </Button>
          </Flex>
          <Separator mb={4} />
          {inProgressRoadmaps.length === 0 ? (
            <Text color="text.muted" fontSize="sm">
              {t('noInProgress')}
            </Text>
          ) : (
            <Stack gap={3}>
              {inProgressRoadmaps.map((roadmap) => (
                <ProgressRow
                  key={roadmap.workspaceId}
                  roadmap={roadmap}
                  progressLabel={t('progress')}
                  onClick={(workspaceId) =>
                    router.push(`/saved/roadmap/${workspaceId}`)
                  }
                />
              ))}
            </Stack>
          )}
        </Box>

        {/* Recent Tests */}
        <Box>
          <Flex justify="space-between" align="center" mb={3}>
            <Heading size="md" color="text.heading">
              {t('recentTestsSection')}
            </Heading>
          </Flex>
          <Separator mb={4} />
          {recentTests.length === 0 ? (
            <Text color="text.muted" fontSize="sm">
              —
            </Text>
          ) : (
            <Stack gap={3}>
              {recentTests.map((test) => (
                <TestRow
                  key={test.attemptId}
                  test={test}
                  scoreLabel={t('score')}
                  inProgressLabel={t('inProgress')}
                />
              ))}
            </Stack>
          )}
        </Box>
      </Grid>
    </Stack>
  );
}
