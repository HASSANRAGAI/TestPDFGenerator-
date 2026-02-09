import type { Metadata } from "next";
import "./globals.css";

export const metadata: Metadata = {
  title: "ACIG General Claims System",
  description: "Insurance claims management system for ACIG",
};

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <html lang="en">
      <body className="antialiased">
        {children}
      </body>
    </html>
  );
}
