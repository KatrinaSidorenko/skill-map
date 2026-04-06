import { Box, Text, HStack, Badge } from '@chakra-ui/react';
import { Handle, Position, NodeProps } from '@xyflow/react';
import { getStatusColor, getNodeTypeColor } from '../../helpers';
import { useAppSelector } from '@/store/hooks';
import { selectPendingIds, selectFailedIds } from '../store';

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
                : getStatusColor(status ?? 'notstarted')
          }
        />
        <Text
          fontSize="sm"
          fontWeight="medium"
          color={isFailed ? 'red.700' : undefined}
        >
          {label ?? ''}
        </Text>
      </HStack>

      {/* Node type badge */}
      {/*{nodeType && (*/}
      {/*  <Badge*/}
      {/*    mt={1}*/}
      {/*    size="xs"*/}
      {/*    colorPalette={typeColor}*/}
      {/*    variant="subtle"*/}
      {/*    borderRadius="full"*/}
      {/*  >*/}
      {/*    {nodeType}*/}
      {/*  </Badge>*/}
      {/*)}*/}

      {/* React Flow handles for connecting nodes */}
      <Handle type="target" position={Position.Top} />
      <Handle type="source" position={Position.Bottom} />
    </Box>
  );
}
