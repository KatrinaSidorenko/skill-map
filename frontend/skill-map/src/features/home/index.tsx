'use client';

import React from 'react';
import {
  Badge,
  Box,
  Button,
  Flex,
  Grid,
  Heading,
  HStack,
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
  FiCompass,
  FiStar,
  FiPlusCircle,
  FiBox,
} from 'react-icons/fi';
import { useRouter } from 'next/navigation';
import { useAppSelector } from '@/store/hooks';
import { selectUser } from '@/features/account/store';
import useLocalization from '@/i18n/useLocalization';
import {
  mockDashboardStats,
  mockInProgressRoadmaps,
  mockRecentTests,
  type RecentTest,
} from './mock';

// ─── Stat Card ────────────────────────────────────────────────────────────────

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
  roadmap: SavedPlainRoadmap;
  progressLabel: string;
  onClick: (id: string) => void;
}

function ProgressRow({ roadmap, progressLabel, onClick }: ProgressRowProps) {
  const pct = Math.min(100, Math.max(0, roadmap.progress));
  return (
    <Box
      bg="bg.section"
      borderRadius="lg"
      p={4}
      boxShadow="sm"
      cursor="pointer"
      _hover={{ boxShadow: 'md', transform: 'translateY(-1px)' }}
      transition="all 0.15s"
      onClick={() => onClick(roadmap.id)}
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
  test: RecentTest;
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

// ─── Quick Action Button ──────────────────────────────────────────────────────

interface QuickActionProps {
  label: string;
  icon: React.ElementType;
  onClick: () => void;
}

function QuickAction({ label, icon, onClick }: QuickActionProps) {
  return (
    <Button
      variant="outline"
      onClick={onClick}
      flexDir="column"
      h="auto"
      py={4}
      px={6}
      gap={2}
      borderColor="border.muted"
      borderWidth="1px"
      borderRadius="xl"
      bg="bg.section"
      _hover={{ bg: 'brand.800', borderColor: 'brand.800', color: 'white' }}
      transition="all 0.2s"
      flex={1}
      minW="100px"
    >
      <Icon as={icon} boxSize={5} color="brand.500" />
      <Text fontSize="xs" fontWeight="semibold" color="text.heading">
        {label}
      </Text>
    </Button>
  );
}

// ─── Dashboard Page ───────────────────────────────────────────────────────────

export default function HomePage() {
  const router = useRouter();
  const user = useAppSelector(selectUser);
  const { getHomeTranslations } = useLocalization();
  const t = getHomeTranslations;

  return (
    <Stack gap={8} pb={8}>
      {/* Stats */}
      <Flex gap={4} wrap="wrap">
        <StatCard
          label={t('savedRoadmaps')}
          value={mockDashboardStats.savedRoadmaps}
          icon={FiBookmark}
        />
        <StatCard
          label={t('testsCompleted')}
          value={mockDashboardStats.testsCompleted}
          icon={FiCheckCircle}
        />
        <StatCard
          label={t('averageScore')}
          value={`${mockDashboardStats.averageScore}${t('percent')}`}
          icon={FiTrendingUp}
        />
        <StatCard
          label={t('activeRoadmaps')}
          value={mockDashboardStats.activeRoadmaps}
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
          {mockInProgressRoadmaps.length === 0 ? (
            <Text color="text.muted" fontSize="sm">
              {t('noInProgress')}
            </Text>
          ) : (
            <Stack gap={3}>
              {mockInProgressRoadmaps.map((roadmap) => (
                <ProgressRow
                  key={roadmap.id}
                  roadmap={roadmap}
                  progressLabel={t('progress')}
                  onClick={(id) => router.push(`/saved/roadmap/${id}`)}
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
          <Stack gap={3}>
            {mockRecentTests.map((test) => (
              <TestRow
                key={test.testId}
                test={test}
                scoreLabel={t('score')}
                inProgressLabel={t('inProgress')}
              />
            ))}
          </Stack>
        </Box>
      </Grid>

      {/* Quick Actions */}
      <Box>
        <Heading size="md" color="text.heading" mb={3}>
          {t('quickActionsSection')}
        </Heading>
        <Separator mb={4} />
        <HStack gap={3} wrap="wrap">
          <QuickAction
            label={t('exploreAction')}
            icon={FiCompass}
            onClick={() => router.push('/explore')}
          />
          <QuickAction
            label={t('savedAction')}
            icon={FiStar}
            onClick={() => router.push('/saved')}
          />
          <QuickAction
            label={t('createAction')}
            icon={FiPlusCircle}
            onClick={() => router.push('/sandbox')}
          />
          <QuickAction
            label={t('sandboxAction')}
            icon={FiBox}
            onClick={() => router.push('/sandbox')}
          />
        </HStack>
      </Box>
    </Stack>
  );
}
