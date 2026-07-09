/**
 * Theme System - Centralized exports
 *
 * This file provides convenient access to all theme-related utilities,
 * tokens, and helpers.
 */

// Design Tokens - Base constants for consistent design
export { spacing, borderRadius, fontSize, lineHeight, fontWeight, shadows, componentSizes, opacity, zIndex } from './design-tokens';

// Theme definitions
export { lightTheme } from './light';
export { darkTheme } from './dark';
export { Theme, ThemeMode, Typography, Fonts } from './theme';

// Base style factory
export { createBaseStyles } from './base-styles';

// Theme utilities for common patterns
export { createCardStyle, createSectionStyle, createButtonStyle, createInputStyle, platformPadding, platformSpacing } from './theme-utils';

// Global utility styles (for backward compatibility)
export { GlobalStyles } from './global-styles';
