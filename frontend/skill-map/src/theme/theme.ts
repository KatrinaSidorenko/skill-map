import { createSystem, defaultConfig, defineConfig } from '@chakra-ui/react';
import { buttonRecipe } from './receips';

const config = defineConfig({
  theme: {
    tokens: {
      fonts: {
        heading: { value: 'var(--font-nunito)' },
        body: { value: 'var(--font-inter)' },
        mono: { value: 'var(--font-fira-code)' },
      },
      colors: {
        brand: {
          20: { value: '#FFFFFB' }, // pure white
          50: { value: '#F1F1E6' }, // off-white
          100: { value: '#EAF3B2' }, // light green
          200: { value: '#9FBAF1' }, // light blue
          300: { value: '#3D568F' }, // deep blue
          400: { value: '#232323' }, // dark gray
          500: { value: '#3D568F' },
          600: { value: '#334674' },
          700: { value: '#2A3659' },
          800: { value: '#20263E' },
          900: { value: '#161A28' },
          950: { value: '#0D0F14' },
        },
      },
    },
    recipes: {
      button: buttonRecipe,
    },
    semanticTokens: {
      colors: {
        bg: {
          page: { value: '{colors.brand.50}' },
          section: { value: '{colors.brand.20}' },
          card: { value: '{colors.brand.200}' },
        },
        text: {
          primary: { value: '{colors.brand.400}' },
          heading: { value: '{colors.brand.800}' },
          muted: { value: '{colors.brand.200}' },
          accent: { value: '{colors.brand.500}' },
        },
        button: {
          solidBg: { value: '{colors.brand.500}' },
          solidText: { value: '{colors.brand.50}' },
          outlineBorder: { value: '{colors.brand.500}' },
          subtleBg: { value: '{colors.brand.100}' },
          subtleText: { value: '{colors.brand.500}' },
        },
        border: {
          default: { value: '{colors.brand.200}' },
          focus: { value: '{colors.brand.800}' },
        },
        states: {
          danger: { value: '{colors.red.500}' },
          warning: { value: '{colors.yellow.400}' },
          success: { value: '{colors.green.500}' },
        },
        sidebar: {
          bg: { value: '{colors.brand.20}' },
          text: { value: '{colors.brand.400}' },
          linkHover: { value: '{colors.brand.100}' },
        },
      },
    },
  },
  globalCss: {
    html: {
      colorPalette: 'brand',
      colorScheme: 'light',
    },
    body: {
      bg: 'brand.50',
      color: 'brand.400',
      minHeight: '100vh',
    },
  },
});

export const system = createSystem(defaultConfig, config);
