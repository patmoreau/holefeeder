import { ExpoSection, type ExpoSectionProps } from './expo/ExpoSection';

export type AppSectionProps = ExpoSectionProps & {};

export const AppSection = (props: AppSectionProps) => <ExpoSection {...props} />;
