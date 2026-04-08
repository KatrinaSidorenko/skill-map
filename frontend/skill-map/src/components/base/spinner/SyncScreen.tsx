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
      <VStack gap={4}>
        <Flex position="relative" alignItems="center" justifyContent="center">
          {/* Outer spinning ring */}
          <Spinner
            color="blue.400"
            animationDuration="1.2s"
            size="xl"
            borderWidth="3px"
          />
          {/* Cloud icon centred inside the spinner */}
          <Flex
            position="absolute"
            alignItems="center"
            justifyContent="center"
          >
            <IoCloudUploadOutline size={22} color="var(--chakra-colors-blue-400)" />
          </Flex>
        </Flex>

        <VStack gap={1}>
          <Text fontWeight="semibold" fontSize="md" color="fg">
            Syncing your changes…
          </Text>
          <Text fontSize="sm" color="fg.muted">
            Sending offline edits to the server
          </Text>
        </VStack>
      </VStack>
    </Flex>
  );
}

