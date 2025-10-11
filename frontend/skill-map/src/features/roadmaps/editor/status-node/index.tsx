import { Box, Text, HStack } from '@chakra-ui/react';
import { Handle, Position, NodeProps } from '@xyflow/react';
import { getStatusColor } from '../../helpers';

export function StatusNode({ data, selected }: NodeProps) {
  const { label, status } = data;

  return (
    <Box
      borderRadius="md"
      borderWidth={1}
      borderColor={selected ? 'brand.200' : 'gray.200'}
      bg="white"
      p={3}
      shadow="sm"
      minW="100px"
      textAlign="center"
    >
      {/* Status indicator + Label */}
      <HStack justify="center" gap={2}>
        <Box
          w="10px"
          h="10px"
          borderRadius="full"
          bg={getStatusColor(status as LearningStatus)}
        />
        <Text fontSize="sm" fontWeight="medium">
          {label as string}
        </Text>
      </HStack>

      {/* React Flow handles for connecting nodes */}
      <Handle type="target" position={Position.Top} />
      <Handle type="source" position={Position.Bottom} />
    </Box>
  );
}
