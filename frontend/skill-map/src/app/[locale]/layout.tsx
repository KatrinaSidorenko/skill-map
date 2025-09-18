import { Provider } from '@/components/ui/provider';
import { routing } from '@/i18n/routing';
import { ReduxProvider } from '@/store/providers';
import { firaCode, inter, nunito } from '@/theme/fonts';
import { hasLocale, NextIntlClientProvider } from 'next-intl';
import { notFound } from 'next/navigation';

export default async function RootLayout({
  children,
  params,
}: {
  children: React.ReactNode;
  params: Promise<{ locale: string }>;
}) {
  const { locale } = await params;
  if (!hasLocale(routing.locales, locale)) {
    notFound();
  }

  return (
    <html
      className={`${nunito.variable} ${inter.variable} ${firaCode.variable}`}
      suppressHydrationWarning
      lang={locale}
    >
      <body suppressHydrationWarning>
        <NextIntlClientProvider>
          <ReduxProvider>
            <Provider>{children}</Provider>
          </ReduxProvider>
        </NextIntlClientProvider>
      </body>
    </html>
  );
}
