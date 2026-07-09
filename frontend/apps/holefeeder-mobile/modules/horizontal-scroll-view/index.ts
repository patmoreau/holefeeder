// Reexport the native module. On web, it will be resolved to HorizontalScrollViewModule.web.ts
// and on native platforms to HorizontalScrollViewModule.ts
export { default } from './src/HorizontalScrollViewModule';
export { default as HorizontalScrollView } from './src/HorizontalScrollView';
export * from './src/HorizontalScrollView.types';
