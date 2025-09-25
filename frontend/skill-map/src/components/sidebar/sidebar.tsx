/* eslint-disable @typescript-eslint/no-explicit-any */
'use client';

import React, { ReactNode } from 'react';
import {
  Box,
  Flex,
  Icon,
  Text,
  Drawer,
  VStack,
  Button,
  Link,
} from '@chakra-ui/react';
import { FiHome, FiCompass, FiStar, FiSettings } from 'react-icons/fi';
import { IconType } from 'react-icons';
import { useSidebar } from './sidebar-context';
import { usePathname, useRouter } from 'next/navigation';
import { AppRouterInstance } from 'next/dist/shared/lib/app-router-context.shared-runtime';

interface LinkItemProps {
  name: string;
  icon: IconType;
  link?: string;
}

const LinkItems: Array<LinkItemProps> = [
  { name: 'Home', icon: FiHome, link: '/home' },
  // { name: 'Trending', icon: FiTrendingUp },
  { name: 'Explore', icon: FiCompass, link: '/explore' },
  { name: 'Favourites', icon: FiStar, link: '/saved' },
  { name: 'Settings', icon: FiSettings, link: '/settings' },
];

export default function SidebarLayout({ children }: { children: ReactNode }) {
  const { open, setOpen } = useSidebar();
  const activePath = usePathname();

  return (
    <Box minH="100vh">
      <SidebarContent
        display={{ base: 'none', md: 'flex' }}
        activePath={activePath}
      />

      {/* Mobile Drawer */}
      <Drawer.Root
        open={open}
        onOpenChange={(e) => setOpen(e.open)}
        placement={'start'}
        size="xs"
      >
        <Drawer.Backdrop />
        <Drawer.Positioner>
          <Drawer.Content width={'60'}>
            <Drawer.CloseTrigger />
            <SidebarContent isDrawer activePath={activePath} />
          </Drawer.Content>
        </Drawer.Positioner>
      </Drawer.Root>

      {/* Page Content */}
      <Box ml={{ base: 0, md: 20 }} h="full" p={4}>
        {children}
      </Box>
    </Box>
  );
}

interface SidebarProps {
  isDrawer?: boolean;
  activePath: string;
  [key: string]: any;
}

const SidebarContent = ({ isDrawer, activePath, ...rest }: SidebarProps) => {
  const router = useRouter();

  return (
    <Flex
      direction="column"
      align="center"
      justify="center"
      bg="sidebar.bg"
      borderRight="1px"
      w={isDrawer ? '60' : '20'} // small width in desktop, full in drawer
      pos="fixed"
      h="full"
      {...rest}
    >
      {isDrawer && (
        <Flex h="20" alignItems="center" px="6" justifyContent="space-between">
          <Text fontSize="2xl" fontWeight="bold">
            Logo
          </Text>
        </Flex>
      )}

      <VStack gap={6} mt={isDrawer ? 4 : 0}>
        {LinkItems.map((link) => (
          <NavItem
            key={link.name}
            icon={link.icon}
            link={link.link}
            isActive={activePath?.includes(link.link || '')}
            router={router}
          >
            {isDrawer ? link.name : null}
          </NavItem>
        ))}
      </VStack>
    </Flex>
  );
};

interface NavItemProps {
  icon: IconType;
  link?: string;
  children?: ReactNode;
  isActive: boolean;
  [key: string]: any;
  router: AppRouterInstance;
}

const NavItem = ({
  icon,
  children,
  link,
  isActive,
  router,
  ...rest
}: NavItemProps) => {
  return (
    <Flex
      align="center"
      borderRadius="md"
      role="group"
      cursor="pointer"
      _hover={{
        bg: 'sidebar.linkHover',
        color: 'text.primary',
      }}
      {...rest}
    >
      <Link
        onClick={() => router.push(link || '/home')}
        bg="transparent"
        padding={0}
        style={{ textDecoration: 'none' }}
      >
        <Button size="xs" bg={isActive ? 'brand.100' : 'transparent'}>
          <Icon fontSize="20" as={icon} />
          {children && <Text ml="3">{children}</Text>}
        </Button>
      </Link>
    </Flex>
  );
};
