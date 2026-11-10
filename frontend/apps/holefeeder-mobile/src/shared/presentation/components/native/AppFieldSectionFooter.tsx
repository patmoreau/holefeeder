import { ExpoFieldSectionFooter, ExpoFieldSectionFooterProps } from '@/shared/presentation/components/native/expo/ExpoFieldSectionFooter';

export type AppFieldSectionFooterProps = ExpoFieldSectionFooterProps & {};

export const AppFieldSectionFooter = (props: AppFieldSectionFooterProps) => {
  return <ExpoFieldSectionFooter {...props} />;
};
