import { Box, Text, HStack } from '@chakra-ui/react';
import { Handle, Position, NodeProps } from '@xyflow/react';
import { getStatusColor } from '../../helpers';
import { useAppSelector } from '@/store/hooks';
import { selectPendingIds, selectFailedIds } from '../store';

export function StatusNode({ id, data, selected }: NodeProps) {
  const { label, status } = data;
  const pendingIds = useAppSelector(selectPendingIds);
  const failedIds = useAppSelector(selectFailedIds);

  const isPending = pendingIds.includes(id);
  const isFailed = failedIds.includes(id);

  return (
    <Box
      borderRadius="md"
      borderWidth={isFailed ? 2 : 1}
      borderColor={
        isFailed ? 'red.400' : selected ? 'brand.200' : 'gray.200'
      }
      bg={isFailed ? 'red.50' : isPending ? 'gray.100' : 'white'}
      p={3}
      shadow="sm"
      minW="100px"
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
      {/* Status indicator + Label */}
      <HStack justify="center" gap={2}>
        <Box
          w="10px"
          h="10px"
          borderRadius="full"
          bg={
            isFailed
              ? 'red.500'
              : isPending
                ? 'gray.400'
                : getStatusColor(status as LearningStatus)
          }
        />
        <Text fontSize="sm" fontWeight="medium" color={isFailed ? 'red.700' : undefined}>
          {label as string}
        </Text>
      </HStack>

      {/* React Flow handles for connecting nodes */}
      <Handle type="target" position={Position.Top} />
      <Handle type="source" position={Position.Bottom} />
    </Box>
  );
}
