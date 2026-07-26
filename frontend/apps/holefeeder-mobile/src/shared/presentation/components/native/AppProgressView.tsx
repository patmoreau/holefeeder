import { ExpoProgressView, type ExpoProgressViewProps } from './expo/ExpoProgressView';

export type AppProgressViewProps = ExpoProgressViewProps & {};

export const AppProgressView = (props: AppProgressViewProps) => <ExpoProgressView {...props} />;
