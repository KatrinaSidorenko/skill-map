import { useAppSelector } from '@/store/hooks';
import { selectRoadmapId, selectSelectedElement } from '../../store';
import { useLazyGetLearningItemMaterialsQuery } from '@/features/roadmaps/api';
import { useEffect } from 'react';
import {
  Badge,
  Box,
  HStack,
  Separator,
  VStack,
  Text,
  Flex,
  Spinner,
  Icon,
  Link,
} from '@chakra-ui/react';
import { AiOutlineFileText, AiOutlineLink } from 'react-icons/ai';
import { FaBook, FaVideo } from 'react-icons/fa';
import { SiCoursera } from 'react-icons/si';

export default function MaterialsContainer() {
  const selectedElement = useAppSelector(selectSelectedElement);
  const roadmapId = useAppSelector(selectRoadmapId);

  const [triggerGetMaterials, { data, isLoading, isFetching }] =
    useLazyGetLearningItemMaterialsQuery();

  useEffect(() => {
    if (!selectedElement || !roadmapId) return;
    triggerGetMaterials({ roadmapId, itemId: selectedElement.id });
  }, [selectedElement, roadmapId, triggerGetMaterials]);

  if (!selectedElement || !roadmapId) return null;

  const materials = data ?? [];

  const getIcon = (type: MaterialType) => {
    switch (type) {
      case 'article':
        return <Icon as={AiOutlineFileText} boxSize={5} color="teal.500" />;
      case 'video':
        return <Icon as={FaVideo} boxSize={5} color="teal.500" />;
      case 'book':
        return <Icon as={FaBook} boxSize={5} color="teal.500" />;
      case 'course':
        return <Icon as={SiCoursera} boxSize={5} color="teal.500" />;
      default:
        return <Icon as={AiOutlineLink} boxSize={5} color="teal.500" />;
    }
  };

  return (
    <VStack align="stretch" gap={4} p={4} width="full">
      <Text fontSize="sm" fontWeight="medium" color="gray.600">
        Resources
      </Text>

      {isLoading || isFetching ? (
        <Flex justify="center" align="center" minH="100px">
          <Spinner size="lg" color="teal.400" />
        </Flex>
      ) : materials.length === 0 ? (
        <Box
          borderWidth="1px"
          borderRadius="lg"
          p={6}
          textAlign="center"
          color="gray.500"
          bg="gray.50"
        >
          No materials found for this topic.
        </Box>
      ) : (
        <VStack align="stretch" gap={3}>
          {materials.map((material: LearningItemMaterial, index: number) => {
            return (
              <Box
                key={material.id}
                borderWidth="1px"
                borderRadius="xl"
                p={3}
                bg="white"
                shadow="sm"
                _hover={{ shadow: 'md', transform: 'scale(1.01)' }}
                transition="all 0.15s ease"
              >
                <HStack justify="space-between" align="center">
                  <HStack gap={3} align="center">
                    <Flex
                      bg="gray.100"
                      borderRadius="full"
                      p={2}
                      justify="center"
                      align="center"
                    >
                      {getIcon(material.type)}
                    </Flex>

                    <VStack align="start" gap={0}>
                      <Link
                        target="_blank"
                        href={material.url}
                        fontWeight="medium"
                        color="teal.600"
                        _hover={{ textDecoration: 'underline' }}
                      >
                        {material.title}
                      </Link>
                      <Text fontSize="xs" color="gray.500">
                        {material.url.replace(/^https?:\/\//, '')}
                      </Text>
                    </VStack>
                  </HStack>

                  <Badge
                    variant="subtle"
                    colorScheme="teal"
                    fontSize="xs"
                    textTransform="capitalize"
                  >
                    {material.type}
                  </Badge>
                </HStack>

                {index < materials.length - 1 && <Separator mt={3} />}
              </Box>
            );
          })}
        </VStack>
      )}
    </VStack>
  );
}
