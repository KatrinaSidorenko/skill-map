'use client';

import {
  Flex,
  IconButton,
  Text,
  Avatar,
  Box,
  HStack,
  Select,
  createListCollection,
  Portal,
} from '@chakra-ui/react';
import { FiMenu } from 'react-icons/fi';
import React, { useTransition } from 'react';
import { useSidebar } from '@/components/sidebar/sidebar-context';
import { useAppSelector } from '@/store/hooks';
import { selectUser } from '@/features/account/store';
import useLocalization from '@/i18n/useLocalization';
import { useLocale } from 'next-intl';
import { usePathname, useRouter } from 'next/navigation';
import { routing } from '@/i18n/routing';

export default function Header() {
  const { setOpen } = useSidebar();
  const user = useAppSelector(selectUser);
  const { getHeaderTranslations } = useLocalization();
  const locale = useLocale();
  const router = useRouter();
  const pathname = usePathname();
  const [isPending, startTransition] = useTransition();

  const localeLabels: Record<string, string> = {
    en: 'EN',
    ua: 'UA',
  };

  const localeOptions = createListCollection({
    items: routing.locales.map((loc) => ({
      label: localeLabels[loc] ?? loc.toUpperCase(),
      value: loc,
    })),
  });

  const handleLocaleChange = (newLocale: string) => {
    if (newLocale === locale) return;

    const segments = pathname.split('/');
    segments[1] = newLocale;
    const newPath = segments.join('/');
    console.log('Navigating to:', newPath);

    startTransition(() => {
      router.push(newPath);
    });
  };

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
      {/* Sidebar menu (mobile) */}
      <IconButton
        variant="outline"
        onClick={() => setOpen(true)}
        aria-label="open menu"
        display={{ base: 'flex', md: 'none' }}
      >
        <FiMenu />
      </IconButton>

      {/* User Info */}
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

      {/* Right side actions */}
      <HStack gap={3}>
        {/* Language Switcher */}
        <Select.Root
          size="xs"
          variant="outline"
          collection={localeOptions}
          value={[locale]}
          onValueChange={(details) => handleLocaleChange(details.value[0])}
          disabled={isPending}
        >
          <Select.HiddenSelect />
          <Select.Control>
            <Select.Trigger minW="110px">
              <Select.ValueText placeholder="Select language" />
            </Select.Trigger>
            <Select.IndicatorGroup>
              <Select.Indicator />
            </Select.IndicatorGroup>
          </Select.Control>

          <Portal>
            <Select.Positioner>
              <Select.Content>
                {localeOptions.items.map((item) => (
                  <Select.Item key={item.value} item={item}>
                    {item.label}
                    <Select.ItemIndicator />
                  </Select.Item>
                ))}
              </Select.Content>
            </Select.Positioner>
          </Portal>
        </Select.Root>
      </HStack>
    </Flex>
  );
}
