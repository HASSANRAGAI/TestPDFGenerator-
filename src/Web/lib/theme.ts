/**
 * ACIG General Claims System Theme Configuration
 * Brand colors and styling based on ACIG identity
 */

export const theme = {
  colors: {
    // Primary ACIG Blue - deep blue used in navigation and primary actions
    primary: {
      DEFAULT: '#1e3a8a', // Deep blue
      dark: '#1e2b5a',
      light: '#2563eb',
      lighter: '#3b82f6',
    },
    // Secondary colors
    secondary: {
      DEFAULT: '#6b7280',
      dark: '#4b5563',
      light: '#9ca3af',
    },
    // Background colors
    background: {
      DEFAULT: '#f9fafb', // Light gray background
      white: '#ffffff',
      gray: '#f3f4f6',
    },
    // Table and border colors
    border: {
      DEFAULT: '#e5e7eb',
      dark: '#d1d5db',
    },
    // Text colors
    text: {
      primary: '#111827',
      secondary: '#6b7280',
      light: '#9ca3af',
      white: '#ffffff',
    },
    // Status colors
    status: {
      success: '#10b981',
      warning: '#f59e0b',
      error: '#ef4444',
      info: '#3b82f6',
    },
  },
  spacing: {
    xs: '0.25rem',   // 4px
    sm: '0.5rem',    // 8px
    md: '1rem',      // 16px
    lg: '1.5rem',    // 24px
    xl: '2rem',      // 32px
    '2xl': '3rem',   // 48px
  },
  borderRadius: {
    sm: '0.25rem',   // 4px
    md: '0.375rem',  // 6px
    lg: '0.5rem',    // 8px
    xl: '0.75rem',   // 12px
  },
  typography: {
    fontFamily: {
      // Arabic-friendly fonts
      sans: [
        '-apple-system',
        'BlinkMacSystemFont',
        'Segoe UI',
        'Roboto',
        'Helvetica Neue',
        'Arial',
        'Noto Sans',
        'sans-serif',
        'Apple Color Emoji',
        'Segoe UI Emoji',
        'Segoe UI Symbol',
        'Noto Color Emoji',
      ].join(','),
      arabic: [
        'Noto Sans Arabic',
        'Arial',
        'sans-serif',
      ].join(','),
    },
    fontSize: {
      xs: '0.75rem',    // 12px
      sm: '0.875rem',   // 14px
      base: '1rem',     // 16px
      lg: '1.125rem',   // 18px
      xl: '1.25rem',    // 20px
      '2xl': '1.5rem',  // 24px
      '3xl': '1.875rem', // 30px
    },
    fontWeight: {
      normal: 400,
      medium: 500,
      semibold: 600,
      bold: 700,
    },
  },
  shadows: {
    sm: '0 1px 2px 0 rgba(0, 0, 0, 0.05)',
    DEFAULT: '0 1px 3px 0 rgba(0, 0, 0, 0.1), 0 1px 2px 0 rgba(0, 0, 0, 0.06)',
    md: '0 4px 6px -1px rgba(0, 0, 0, 0.1), 0 2px 4px -1px rgba(0, 0, 0, 0.06)',
    lg: '0 10px 15px -3px rgba(0, 0, 0, 0.1), 0 4px 6px -2px rgba(0, 0, 0, 0.05)',
    xl: '0 20px 25px -5px rgba(0, 0, 0, 0.1), 0 10px 10px -5px rgba(0, 0, 0, 0.04)',
  },
};

export type Theme = typeof theme;
