import { defineRecipe } from '@chakra-ui/react';

export const buttonRecipe = defineRecipe({
  base: {
    display: 'inline-flex',
    alignItems: 'center',
    justifyContent: 'center',
    gap: '2',
    fontFamily: 'body',
    fontWeight: 'medium',
    borderRadius: 'lg',
    cursor: 'pointer',
    transition: 'all 0.15s ease',
    _disabled: { opacity: 0.5, cursor: 'not-allowed', pointerEvents: 'none' },
  },
  variants: {
    // ← 'variant' (not 'visual') so <Button variant="solid"> works correctly
    variant: {
      // Primary teal — navigation, save, default actions
      solid: {
        bg: 'button.solidBg',
        color: 'button.solidText',
        _hover: {
          bg: '{colors.brand.600}',
          transform: 'translateY(-1px)',
          boxShadow: 'sm',
        },
        _active: { bg: '{colors.brand.700}', transform: 'translateY(0)' },
      },
      // Outline teal — secondary actions
      outline: {
        bg: 'transparent',
        color: 'button.solidBg',
        borderWidth: '1.5px',
        borderColor: 'button.outlineBorder',
        _hover: { bg: 'button.subtleBg' },
        _active: { bg: 'button.subtleBg', filter: 'brightness(0.95)' },
      },
      // Ghost teal — tertiary / toolbar actions
      ghost: {
        bg: 'transparent',
        color: 'button.subtleText',
        _hover: { bg: 'button.subtleBg' },
        _active: { bg: 'button.subtleBg', filter: 'brightness(0.95)' },
      },
      // Subtle — de-emphasized, low-contrast
      subtle: {
        bg: 'button.subtleBg',
        color: 'button.subtleText',
        _hover: { filter: 'brightness(0.96)' },
        _active: { filter: 'brightness(0.92)' },
      },
      // 🌟 Accent lime — hero CTAs ("Open Editor", "Publish", primary page action)
      accent: {
        bg: 'button.accentBg',
        color: 'button.accentText',
        fontWeight: 'semibold',
        _hover: {
          bg: '{colors.brand.300}',
          transform: 'translateY(-1px)',
          boxShadow: 'sm',
        },
        _active: { bg: '{colors.brand.300}', transform: 'translateY(0)' },
      },
    },
    size: {
      xs: { px: 2,  py: 1,   fontSize: 'xs', h: '6'  },
      sm: { px: 3,  py: 1.5, fontSize: 'sm', h: '8'  },
      md: { px: 4,  py: 2,   fontSize: 'md', h: '10' },
      lg: { px: 6,  py: 3,   fontSize: 'lg', h: '12' },
    },
  },
  defaultVariants: {
    variant: 'solid',
    size: 'md',
  },
});
