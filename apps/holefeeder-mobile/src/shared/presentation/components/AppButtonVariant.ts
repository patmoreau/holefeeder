export const AppButtonVariant = {
  primary: 'primary',
  secondary: 'secondary',
  destructive: 'destructive',
  link: 'link',
};

export type AppButtonVariant = (typeof AppButtonVariant)[keyof typeof AppButtonVariant];
