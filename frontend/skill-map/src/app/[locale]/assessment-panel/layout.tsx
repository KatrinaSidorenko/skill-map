import { Box } from '@chakra-ui/react';

export default function AssessmentPanelLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  return (
    <Box
      display="flex"
      justifyContent="center"
      bg="bg.section"
      alignItems="center"
      minHeight="100vh"
      padding={10}
    >
      {children}
    </Box>
  );
}
