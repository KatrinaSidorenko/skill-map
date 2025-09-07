import { Sidebar } from '@/components/sidenav/sidebar';
import { SidebarProvider } from '@/components/sidenav/sidebar-context';
import { Box, Flex, Stack } from '@chakra-ui/react';
import { TopSection } from './top-section';

export default function HomePage() {
  return (
    <SidebarProvider>
      <Flex minH="100dvh">
        <Sidebar />

        <Box flex="1">
          <Stack h="full">
            <TopSection />
          </Stack>
        </Box>
      </Flex>
    </SidebarProvider>
  );
}
