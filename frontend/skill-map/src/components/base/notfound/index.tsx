import useLocalization from '@/i18n/useLocalization';
import { Flex } from '@chakra-ui/react';

export default function ContentNotFoundScreen() {
  const { getGeneralTranslations } = useLocalization();
  return (
    <Flex w="full" h="full" alignItems="center" justifyContent="center">
      <div>{getGeneralTranslations('notFound')} :( </div>
    </Flex>
  );
}
