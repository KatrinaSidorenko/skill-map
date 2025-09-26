import { Box } from '@chakra-ui/react';

export default function Container({
  children,
  isSection = true,
}: {
  children?: React.ReactNode;
  isSection?: boolean;
}) {
  return (
    <Box
      px={6}
      py={4}
      borderRadius="lg"
      bg={isSection ? 'bg.section' : 'bg.page'}
      w="full"
      h="full"
    >
      {children}
    </Box>
  );
}
