import { Provider } from '@/components/ui/provider';
import { firaCode, inter, nunito } from '@/theme/fonts';

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <html className={`${nunito.variable} ${inter.variable} ${firaCode.variable}`} suppressHydrationWarning>
      <body suppressHydrationWarning>
        <Provider>{children}</Provider>
      </body>
    </html>
  );
}
