import { ExpoFieldSection, ExpoFieldSectionProps } from '@/shared/presentation/components/app/expo/ExpoFieldSection';

export type AppFieldSectionProps = ExpoFieldSectionProps & {};

export const AppFieldSection = (props: AppFieldSectionProps) => {
  return <ExpoFieldSection {...props} />;
};
