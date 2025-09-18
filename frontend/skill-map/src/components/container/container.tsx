import { Box } from '@chakra-ui/react';

export default function Container({
  children,
}: {
  children?: React.ReactNode;
}) {
  return (
    <Box
      px={6}
      py={4}
      borderRadius="lg"
      bg="bg.section"
      w="full"
      h="full"
    >
      {children}
    </Box>
  );
}
