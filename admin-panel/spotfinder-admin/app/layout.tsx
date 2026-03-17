import type { Metadata } from "next";
import "./globals.css";

export const metadata: Metadata = {
  title: "SpotFinder Admin",
  description: "SpotFinder Yönetim Paneli",
};

export default function RootLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  return (
    <html lang="tr">
      <body className="bg-gray-100 text-gray-900 antialiased">
        {children}
      </body>
    </html>
  );
}
