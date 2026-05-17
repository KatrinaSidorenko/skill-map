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

      // ─── Skill-Map Color System ─────────────────────────────────────
      //
      //  Story: clean white canvas  +  lime green = growth/achievement
      //         teal = knowledge/navigation  (bridges lime & dark text)
      //
      //  brand.20–50    → backgrounds (white, section gray)
      //  brand.100–300  → lime-green family (accent, CTA, hover)
      //  brand.400      → primary text (near-black)
      //  brand.500–950  → teal (interactive: buttons, links, focus)
      //  brand.1000     → error red
      // ────────────────────────────────────────────────────────────────
      colors: {
        brand: {
          20: { value: '#F7F9F7' }, // section / card bg  (pale green-tinted gray)
          50: { value: '#FFFFFF' }, // page bg
          100: { value: '#E8FDD4' }, // mint tint           (subtle accent fills)
          200: { value: '#B9FF66' }, // 🌟 lime green       (signature CTA)
          300: { value: '#95E044' }, // lime green hover
          400: { value: '#1A1A1A' }, // primary text        (near-black)
          500: { value: '#127168' }, // teal                (primary interactive)
          600: { value: '#156763' }, // teal hover
          700: { value: '#15514d' }, // teal active/pressed
          800: { value: '#134E4A' }, // teal dark           (headings, emphasis)
          900: { value: '#1A2E2C' }, // deep teal-black
          950: { value: '#0D1716' }, // darkest
          1000: { value: '#EF4444' }, // error red
        },
      },
    },
    recipes: {
      button: buttonRecipe,
    },
    semanticTokens: {
      colors: {
        bg: {
          page: { value: '{colors.brand.50}' }, // white
          section: { value: '{colors.brand.20}' }, // pale green-gray  (cards)
          accent: { value: '{colors.brand.100}' }, // mint tint        (highlights)
          primaryAccent: { value: '{colors.brand.200}' }, // lime green
        },
        text: {
          primary: { value: '{colors.brand.400}' }, // #1A1A1A  near-black
          heading: { value: '{colors.brand.800}' }, // dark teal
          muted: { value: '{colors.gray.500}' }, // ✅ neutral gray (was broken lime!)
          secondary: { value: '{colors.gray.600}' }, // medium gray
          accent: { value: '{colors.brand.200}' }, // lime green (on dark bg)
          onAccent: { value: '{colors.brand.800}' }, // dark teal ON lime bg
        },
        button: {
          solidBg: { value: '{colors.brand.500}' }, // teal
          solidText: { value: '{colors.brand.50}' }, // white
          outlineBorder: { value: '{colors.brand.500}' }, // teal
          subtleBg: { value: '{colors.brand.100}' }, // mint tint
          subtleText: { value: '{colors.brand.500}' }, // teal
          accentBg: { value: '{colors.brand.200}' }, // lime
          accentText: { value: '{colors.brand.800}' }, // dark teal on lime
        },
        border: {
          default: { value: '{colors.brand.20}' }, // subtle gray-green
          muted: { value: '{colors.gray.200}' }, // neutral gray border
          focus: { value: '{colors.brand.500}' }, // teal focus ring
        },
        states: {
          danger: { value: '{colors.brand.1000}' }, // red
          warning: { value: '{colors.yellow.500}' },
          success: { value: '{colors.brand.200}' }, // lime = success ✅
          info: { value: '{colors.brand.500}' }, // teal = info ℹ️
        },
        sidebar: {
          bg: { value: '{colors.brand.20}' },
          text: { value: '{colors.brand.400}' },
          linkHover: { value: '{colors.brand.100}' },
          active: { value: '{colors.brand.200}' }, // lime active link
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
