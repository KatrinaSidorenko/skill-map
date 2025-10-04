'use client';
import {
  Flex,
  IconButton,
  Text,
  Avatar,
  Button,
  Icon,
  Box,
} from '@chakra-ui/react';
import { useSidebar } from '@/components/sidebar/sidebar-context';
import { FiMenu } from 'react-icons/fi';
import React from 'react';
import { useAppSelector } from '@/store/hooks';
import { selectUser } from '@/features/account/store';
import useLocalization from '@/i18n/useLocalization';
import { CiLogout } from 'react-icons/ci';
import { useAuth } from '@/features/account/useAuthContext';

export default function Header() {
  const { setOpen } = useSidebar();
  const user = useAppSelector(selectUser);
  const { getHeaderTranslations } = useLocalization();
  const { logout } = useAuth();

  return (
    <Flex
      as="header"
      direction="row"
      align="center"
      justify="space-between"
      px={4}
      py={1}
      bg="transparent"
      borderRadius="lg"
      w="full"
    >
      <IconButton
        variant="outline"
        onClick={() => setOpen(true)}
        aria-label="open menu"
        display={{ base: 'flex', md: 'none' }}
      >
        <FiMenu />
      </IconButton>
      <Box display="flex" alignItems="center" gap={3}>
        <Avatar.Root>
          <Avatar.Fallback name={user?.username ?? ''} />
          <Avatar.Image
            src={user?.avatarUrl ?? 'https://avatar.iran.liara.run/public'}
          />
        </Avatar.Root>
        <Text fontSize="lg" fontWeight="bold" color="text.heading">
          {user
            ? `${getHeaderTranslations('welcome')}, ${user.username}`
            : getHeaderTranslations('welcome')}
        </Text>
      </Box>

      <>
        <Button size="xs" onClick={() => logout()}>
          <Icon as={CiLogout} />
        </Button>
      </>
    </Flex>
  );
}
