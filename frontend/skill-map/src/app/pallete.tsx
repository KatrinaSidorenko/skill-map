'use client';

import { Box, VStack, Text, SimpleGrid } from '@chakra-ui/react';

const colors = {
  50: '#FDFDF5', // off-white
  100: '#EAF3B2', // light green
  200: '#9FBAF1', // light blue
  300: '#3D568F', // deep blue
  400: '#232323', // dark gray
  500: '#3D568F',
  600: '#334674',
  700: '#2A3659',
  800: '#20263E',
  900: '#161A28',
  950: '#0D0F14',
};

export default function ColorPalette() {
  return (
    <SimpleGrid columns={{ base: 2, md: 4 }} gap={4} p={8}>
      {Object.entries(colors).map(([name, hex]) => (
        <Box
          key={name}
          bg={hex}
          height="120px"
          borderRadius="md"
          display="flex"
          alignItems="center"
          justifyContent="center"
          flexDirection="column"
          color={parseInt(name) >= 400 ? 'white' : 'black'} // contrast text
        >
          <Text fontWeight="bold">{name}</Text>
          <Text>{hex}</Text>
        </Box>
      ))}
    </SimpleGrid>
  );
}
