import useLocalization from '@/i18n/useLocalization';
import { Flex } from '@chakra-ui/react';

export default function ContentNotFoundScreen() {
  const { getGeneralTranslations } = useLocalization();
  return (
    <Flex w="100%" h="100%" alignItems="center" justifyContent="center">
      <div>{getGeneralTranslations('notFound')} :( </div>
    </Flex>
  );
}
