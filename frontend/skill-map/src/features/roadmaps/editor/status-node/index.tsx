import { Box, Text, VStack, Badge } from '@chakra-ui/react';
import { Handle, Position, NodeProps } from '@xyflow/react';
import { getStatusColor, getNodeTypeColor } from '../../helpers';
import { useAppSelector } from '@/store/hooks';
import { selectPendingIds, selectFailedIds } from '../store';

const STATUS_LABELS: Record<LearningStatus, string> = {
  notstarted: 'Not Started',
  inprogress: 'In Progress',
  completed: 'Completed',
  skip: 'Skip',
  repeat: 'Repeat',
  upcoming: 'Upcoming',
};

export function StatusNode({ id, data, selected }: NodeProps) {
  const label = data.label as string | undefined;
  const status = data.status as LearningStatus | undefined;
  const nodeType = data.nodeType as LearningItemType | undefined;
  const pendingIds = useAppSelector(selectPendingIds);
  const failedIds = useAppSelector(selectFailedIds);

  const isPending = pendingIds.includes(id);
  const isFailed = failedIds.includes(id);

  const typeColor = getNodeTypeColor(
    (nodeType as LearningItemType) ?? 'subtopic',
  );
  const topBorderColor = `${typeColor}.400`;
  const effectiveStatus: LearningStatus = status ?? 'notstarted';
  const statusColor = isFailed ? 'red' : isPending ? 'gray' : getStatusColor(effectiveStatus);
  const statusLabel = isFailed ? 'Failed' : isPending ? 'Saving…' : STATUS_LABELS[effectiveStatus];

  return (
    <Box
      borderRadius="lg"
      borderWidth={isFailed ? 2 : 1}
      borderColor={isFailed ? 'red.400' : selected ? 'brand.500' : 'gray.200'}
      borderTopWidth={3}
      borderTopColor={isFailed ? 'red.400' : topBorderColor}
      bg={isFailed ? 'red.50' : isPending ? 'gray.100' : 'white'}
      p={3}
      shadow="md"
      minW="140px"
      textAlign="center"
      opacity={isPending ? 0.65 : 1}
      css={
        isPending && !isFailed
          ? {
              '@keyframes pending-pulse': {
                '0%, 100%': { opacity: 0.65 },
                '50%': { opacity: 0.35 },
              },
              animation: 'pending-pulse 1.4s ease-in-out infinite',
            }
          : undefined
      }
    >
      <VStack gap={2} align="center">
        {/* Label */}
        <Text
          fontSize="sm"
          fontWeight="semibold"
          color={isFailed ? 'red.700' : 'gray.800'}
          lineHeight="short"
        >
          {label ?? ''}
        </Text>

        {/* Status badge */}
        <Badge
          colorPalette={statusColor}
          variant="subtle"
          size="sm"
          borderRadius="full"
          px={2}
        >
          {statusLabel}
        </Badge>
      </VStack>

      <Handle type="target" position={Position.Top} />
      <Handle type="source" position={Position.Bottom} />
    </Box>
  );
}
