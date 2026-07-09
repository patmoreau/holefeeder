import { ExpoSpacer, ExpoSpacerProps } from './expo/ExpoSpacer';

export type AppSpacerProps = ExpoSpacerProps & {};

export const AppSpacer = (props: AppSpacerProps) => {
  return <ExpoSpacer {...props} />;
};
