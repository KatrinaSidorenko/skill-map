import { Box, Text, VStack } from '@chakra-ui/react';
import { Handle, Position, NodeProps } from '@xyflow/react';
import { getNodeTypeColor } from '../../helpers';
import { useAppSelector } from '@/store/hooks';
import { selectPendingIds, selectFailedIds } from '../store';

export function CreatorNode({ id, data, selected }: NodeProps) {
  const label = data.label as string | undefined;
  const nodeType = data.nodeType as LearningItemType | undefined;
  const pendingIds = useAppSelector(selectPendingIds);
  const failedIds = useAppSelector(selectFailedIds);

  const isPending = pendingIds.includes(id);
  const isFailed = failedIds.includes(id);

  const typeColor = getNodeTypeColor((nodeType as LearningItemType) ?? 'subtopic');
  const topBorderColor = `${typeColor}.400`;

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
      <VStack gap={1} align="center">
        <Text
          fontSize="sm"
          fontWeight="semibold"
          color={isFailed ? 'red.700' : 'gray.800'}
          lineHeight="short"
        >
          {label ?? ''}
        </Text>
      </VStack>

      <Handle type="target" position={Position.Top} />
      <Handle type="source" position={Position.Bottom} />
    </Box>
  );
}

