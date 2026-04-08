'use client';

import { Box, VStack, HStack, Text, SimpleGrid, Separator } from '@chakra-ui/react';

const sections = [
	{
		label: 'Backgrounds',
		swatch: [
			{ token: 'brand.50', hex: '#FFFFFF', name: 'Page bg' },
			{ token: 'brand.20', hex: '#F7F9F7', name: 'Section / Card' },
			{ token: 'brand.100', hex: '#E8FDD4', name: 'Accent fill' },
		],
	},
	{
		label: '🌟 Lime Green — Growth / CTA',
		swatch: [
			{ token: 'brand.200', hex: '#B9FF66', name: 'Primary CTA' },
			{ token: 'brand.300', hex: '#95E044', name: 'Hover / Pressed' },
		],
	},
	{
		label: 'Teal — Knowledge / Interactive',
		swatch: [
			{ token: 'brand.500', hex: '#0D9488', name: 'Primary action' },
			{ token: 'brand.600', hex: '#0F766E', name: 'Hover' },
			{ token: 'brand.700', hex: '#115E59', name: 'Active' },
			{ token: 'brand.800', hex: '#134E4A', name: 'Heading / Dark' },
			{ token: 'brand.900', hex: '#1A2E2C', name: 'Deep' },
			{ token: 'brand.950', hex: '#0D1716', name: 'Darkest' },
		],
	},
	{
		label: 'Text',
		swatch: [
			{ token: 'brand.400', hex: '#1A1A1A', name: 'Primary text' },
			{ token: 'gray.600', hex: '#4B5563', name: 'Secondary text' },
			{ token: 'gray.500', hex: '#6B7280', name: 'Muted text' },
		],
	},
	{
		label: 'State',
		swatch: [
			{ token: 'brand.200', hex: '#B9FF66', name: 'Success (lime)' },
			{ token: 'brand.500', hex: '#0D9488', name: 'Info (teal)' },
			{ token: 'yellow.500', hex: '#EAB308', name: 'Warning' },
			{ token: 'brand.1000', hex: '#EF4444', name: 'Error' },
		],
	},
];

function Swatch({
	token,
	hex,
	name,
	dark,
}: {
	token: string;
	hex: string;
	name: string;
	dark?: boolean;
}) {
	return (
		<VStack gap={1} align="start">
			<Box
				bg={hex}
				h="64px"
				w="full"
				borderRadius="lg"
				borderWidth="1px"
				borderColor="gray.200"
				boxShadow="xs"
			/>
			<Text fontSize="xs" fontWeight="semibold" color={dark ? 'white' : 'brand.400'}>
				{name}
			</Text>
			<Text fontSize="xs" color="gray.500" fontFamily="mono">
				{hex}
			</Text>
			<Text fontSize="xs" color="gray.400" fontFamily="mono">
				{token}
			</Text>
		</VStack>
	);
}

export default function ColorPalette() {
	return (
		<VStack align="stretch" gap={8} p={8} maxW="860px" mx="auto">
			<VStack align="start" gap={1}>
				<Text fontSize="2xl" fontWeight="bold" color="brand.800">
					Skill-Map Design Tokens
				</Text>
				<Text fontSize="sm" color="gray.500">
					White canvas · Lime green = growth · Teal = knowledge
				</Text>
			</VStack>

			{sections.map(({ label, swatch }) => (
				<VStack key={label} align="stretch" gap={3}>
					<HStack>
						<Text fontSize="sm" fontWeight="semibold" color="brand.700">
							{label}
						</Text>
						<Separator flex="1" borderColor="gray.200" />
					</HStack>
					<SimpleGrid columns={{ base: 2, sm: 3, md: 4, lg: 6 }} gap={4}>
						{swatch.map((s) => (
							<Swatch key={s.token} {...s} />
						))}
					</SimpleGrid>
				</VStack>
			))}
		</VStack>
	);
}
