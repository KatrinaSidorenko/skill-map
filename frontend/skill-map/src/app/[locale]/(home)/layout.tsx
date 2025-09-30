'use client';

import Container from '@/components/container/container';
import Header from '@/components/base/header/header';
import SidebarLayout from '@/components/sidebar/sidebar';
import { SidebarProvider } from '@/components/sidebar/sidebar-context';
import { Flex } from '@chakra-ui/react';

export default function HomeLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <SidebarProvider>
      <SidebarLayout>
        <Flex bg="bg.page" gap={4} height="100vh" direction="column">
          <Header />
          <Container>{children}</Container>
        </Flex>
      </SidebarLayout>
    </SidebarProvider>
  );
}
