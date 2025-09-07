import { defineRecipe } from '@chakra-ui/react';

export const buttonRecipe = defineRecipe({
  base: {
    display: 'flex',
    alignItems: 'center',
    justifyContent: 'center',

    borderRadius: 'md',
    transition: 'background-color 0.2s, color 0.2s',
  },
  variants: {
    visual: {
      solid: {
        color: 'brand.50',
        bg: 'brand.500',
        _hover: { bg: 'brand.600' },
        _active: { bg: 'brand.700' },
      },
      outline: {
        color: 'brand.50',
        borderWidth: '1px',
        borderColor: 'brand.300',
        _hover: { bg: 'brand.50' },
      },
      ghost: { _hover: { bg: 'brand.50' } },
    },
    size: {
      sm: { padding: '2', fontSize: '12px' },
      md: { padding: '4', fontSize: '16px' },
      lg: { padding: '6', fontSize: '20px' },
    },
  },
  defaultVariants: {
    visual: 'solid',
    size: 'md',
  },
});

export const cardRecipe = defineRecipe({
  base: {
    borderRadius: 'md',
    padding: '6',
    boxShadow: 'md',
    bg: 'brand.backgroundLight',
  },
  variants: {
    visual: {
      light: { bg: 'brand.backgroundLight', color: 'text.default' },
      dark: { bg: 'brand.backgroundDark', color: 'text.default' },
    },
  },
  defaultVariants: { visual: 'light' },
});

export const drawerRecipe = defineRecipe({
  base: {
    // Applies to the drawer container
    borderRadius: 'lg',
    bg: 'brand.backgroundLight',
    color: 'text.default',
    padding: '6',
    width: '100%', // full width on mobile
    maxWidth: 'md', // max width on desktop
    boxShadow: 'lg',
  },
  variants: {
    visual: {
      light: {
        bg: 'brand.backgroundLight',
        color: 'text.default',
        headerColor: 'text.heading',
        footerColor: 'text.default',
      },
      dark: {
        bg: 'brand.backgroundDark',
        color: 'text.default',
        headerColor: 'text.accent',
        footerColor: 'text.muted',
      },
    },
    size: {
      sm: { maxWidth: 'sm', padding: '4' },
      md: { maxWidth: 'md', padding: '6' },
      lg: { maxWidth: 'lg', padding: '8' },
    },
  },
  defaultVariants: {
    visual: 'light',
    size: 'md',
  },
});
