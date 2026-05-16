/* eslint-disable @typescript-eslint/no-explicit-any */
'use client';

import React, { ReactNode } from 'react';
import { Box, Flex, Icon, Text, Drawer, VStack, Heading } from '@chakra-ui/react';
import {
  FiHome,
  FiCompass,
  FiStar,
  FiSettings,
  FiLogOut,
} from 'react-icons/fi';
import { IconType } from 'react-icons';
import { useSidebar } from './sidebar-context';
import { usePathname, useRouter } from 'next/navigation';
import { AppRouterInstance } from 'next/dist/shared/lib/app-router-context.shared-runtime';
import { useAuth } from '@/features/account/useAuthContext';

interface LinkItemProps {
  name: string;
  icon: IconType;
  link?: string;
}

const LinkItems: Array<LinkItemProps> = [
  { name: 'Home', icon: FiHome, link: '/home' },
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
  const { logout } = useAuth();

  return (
    <Flex
      direction="column"
      align="center"
      bg="brand.800"
      borderRight="none"
      w={isDrawer ? '60' : '20'}
      pos="fixed"
      h="full"
      boxShadow="lg"
      {...rest}
    >
      {/* ── Logo ── */}
      {isDrawer ? (
        <Flex h="16" alignItems="center" px="6" w="full" flexShrink={0} gap={3}>
          <Box
            w="28px"
            h="28px"
            borderRadius="md"
            bg="brand.200"
            display="flex"
            alignItems="center"
            justifyContent="center"
            flexShrink={0}
          >
            <Box w="12px" h="12px" borderRadius="sm" bg="brand.800" />
          </Box>
          <Heading fontSize="lg" fontWeight="800" color="brand.200" letterSpacing="tight">
            SkillMap
          </Heading>
        </Flex>
      ) : (
        <Flex h="16" alignItems="center" justifyContent="center" w="full" flexShrink={0}>
          <Box
            w="32px"
            h="32px"
            borderRadius="md"
            bg="brand.200"
            display="flex"
            alignItems="center"
            justifyContent="center"
          >
            <Box w="14px" h="14px" borderRadius="sm" bg="brand.800" />
          </Box>
        </Flex>
      )}

      {/* ── Nav items ── */}
      <Flex
        direction="column"
        justify="center"
        align="center"
        flex="1"
        w="full"
        px={2}
      >
        <VStack gap={2} w={isDrawer ? 'full' : 'auto'} align="center">
          {LinkItems.map((link) => (
            <NavItem
              key={link.name}
              icon={link.icon}
              link={link.link}
              isActive={activePath?.includes(link.link || '')}
              isDrawer={isDrawer}
              router={router}
            >
              {isDrawer ? link.name : null}
            </NavItem>
          ))}
        </VStack>
      </Flex>

      {/* ── Logout ── */}
      <Box
        pb={4}
        flexShrink={0}
        w={isDrawer ? 'full' : 'auto'}
        px={isDrawer ? 2 : 0}
      >
        <Flex
          align="center"
          gap={3}
          px={3}
          py={2}
          borderRadius="lg"
          cursor="pointer"
          color="brand.100"
          _hover={{ bg: 'red.900', color: 'red.300' }}
          transition="all 0.15s ease"
          onClick={() => logout()}
          w={isDrawer ? 'full' : 'auto'}
          justify={isDrawer ? 'flex-start' : 'center'}
        >
          <Icon boxSize={5} as={FiLogOut} flexShrink={0} />
          {isDrawer && (
            <Text fontSize="sm" fontWeight="medium">
              Logout
            </Text>
          )}
        </Flex>
      </Box>
    </Flex>
  );
};

interface NavItemProps {
  icon: IconType;
  link?: string;
  children?: ReactNode;
  isActive: boolean;
  isDrawer?: boolean;
  [key: string]: any;
  router: AppRouterInstance;
}

const NavItem = ({
  icon,
  children,
  link,
  isActive,
  isDrawer,
  router,
  ...rest
}: NavItemProps) => {
  return (
    <Flex
      align="center"
      justify={isDrawer ? 'flex-start' : 'center'}
      gap={3}
      px={3}
      py={2}
      borderRadius="lg"
      cursor="pointer"
      role="group"
      bg={isActive ? 'brand.200' : 'transparent'}
      color={isActive ? 'brand.800' : 'brand.100'}
      _hover={{
        bg: isActive ? 'brand.300' : 'brand.700',
        color: isActive ? 'brand.800' : 'white',
      }}
      transition="all 0.15s ease"
      onClick={() => router.push(link || '/home')}
      w={isDrawer ? 'full' : 'auto'}
      minW="10"
      {...rest}
    >
      <Icon boxSize={5} as={icon} flexShrink={0} />
      {children && (
        <Text fontSize="sm" fontWeight={isActive ? 'semibold' : 'medium'}>
          {children}
        </Text>
      )}
    </Flex>
  );
};
