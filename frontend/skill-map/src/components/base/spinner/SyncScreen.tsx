import { Flex, Spinner, Text, VStack } from '@chakra-ui/react';
import { IoCloudUploadOutline } from 'react-icons/io5';

/**
 * Full-screen overlay shown while offline changes are being synced to the
 * server before the editor canvas is loaded.
 */
export default function SyncScreen() {
  return (
    <Flex
      w="full"
      h="full"
      minH="60vh"
      alignItems="center"
      justifyContent="center"
    >
      <VStack gap={5}>
        <Flex position="relative" alignItems="center" justifyContent="center">
          {/* Outer spinning ring */}
          <Spinner
            color="brand.800"
            animationDuration="1.1s"
            size="xl"
            borderWidth="3px"
          />
          {/* Cloud icon centred inside the spinner */}
          <Flex
            position="absolute"
            alignItems="center"
            justifyContent="center"
          >
            <IoCloudUploadOutline size={22} color="var(--chakra-colors-brand-200)" />
          </Flex>
        </Flex>

        <VStack gap={1}>
          <Text fontWeight="700" fontSize="md" color="text.heading">
            Syncing your changes…
          </Text>
          <Text fontSize="sm" color="text.muted">
            Sending offline edits to the server
          </Text>
        </VStack>
      </VStack>
    </Flex>
  );
}
