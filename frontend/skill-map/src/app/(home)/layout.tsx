'use client';
import Container from '@/components/container/container';
import Header from '@/components/header/header';
import SidebarLayout from '@/components/sidebar/sidebar';
import { SidebarProvider } from '@/components/sidebar/sidebar-context';
import { Flex } from '@chakra-ui/react';
import { usePathname } from 'next/navigation';

export default function HomeLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  const pathname = usePathname();
  const isHome = pathname === '/';
  return (
    <SidebarProvider>
      <Flex minH="100dvh" direction="column">
        <SidebarLayout>
          <Flex direction="column" bg="bg.page" gap={4}>
            {isHome && <Header />}
            <Container>{children}</Container>
          </Flex>
        </SidebarLayout>
      </Flex>
    </SidebarProvider>
  );
}
