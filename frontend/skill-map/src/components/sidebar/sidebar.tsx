/* eslint-disable @typescript-eslint/no-explicit-any */
'use client';

import React, { ReactNode } from 'react';
import { Box, Flex, Icon, Text, Drawer, VStack } from '@chakra-ui/react';
import { FiHome, FiCompass, FiStar, FiSettings } from 'react-icons/fi';
import { IconType } from 'react-icons';
import { useSidebar } from './sidebar-context';

interface LinkItemProps {
  name: string;
  icon: IconType;
}

const LinkItems: Array<LinkItemProps> = [
  { name: 'Home', icon: FiHome },
  // { name: 'Trending', icon: FiTrendingUp },
  { name: 'Explore', icon: FiCompass },
  { name: 'Favourites', icon: FiStar },
  { name: 'Settings', icon: FiSettings },
];

export default function SidebarLayout({ children }: { children: ReactNode }) {
  const { open, setOpen } = useSidebar();

  return (
    <Box minH="100vh">
      <SidebarContent display={{ base: 'none', md: 'flex' }} />

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
            <SidebarContent isDrawer />
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
  [key: string]: any;
}

const SidebarContent = ({ isDrawer, ...rest }: SidebarProps) => {
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
          <NavItem key={link.name} icon={link.icon}>
            {isDrawer ? link.name : null}
          </NavItem>
        ))}
      </VStack>
    </Flex>
  );
};

interface NavItemProps {
  icon: IconType;
  children?: ReactNode;
  [key: string]: any;
}

const NavItem = ({ icon, children, ...rest }: NavItemProps) => {
  return (
    <Flex
      align="center"
      p="2"
      borderRadius="md"
      role="group"
      cursor="pointer"
      _hover={{
        bg: 'sidebar.linkHover',
        color: 'text.primary',
      }}
      {...rest}
    >
      <Icon fontSize="20" as={icon} />
      {children && <Text ml="3">{children}</Text>}
    </Flex>
  );
};
