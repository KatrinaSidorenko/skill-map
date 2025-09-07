import Container from '@/components/container/container';
import Header from '@/components/header/header';
import SidebarLayout, { MobileNav } from '@/components/sidenav/sidebar';
import { SidebarProvider } from '@/components/sidenav/sidebar-context';
import { Flex } from '@chakra-ui/react';

export default function HomeLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <SidebarProvider>
      <Flex minH="100dvh" direction="column">
        <MobileNav />
        <SidebarLayout>
          <Flex direction="column" bg="bg.page" gap={4}>
            <Header />
            <Container>{children}</Container>
          </Flex>
        </SidebarLayout>
      </Flex>
    </SidebarProvider>
  );
}
