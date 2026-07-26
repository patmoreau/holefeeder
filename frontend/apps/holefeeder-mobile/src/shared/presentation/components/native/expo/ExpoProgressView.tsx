import { ProgressView, type ProgressViewProps } from '@expo/ui/swift-ui';

export type ExpoProgressViewProps = ProgressViewProps;

export const ExpoProgressView = (props: ExpoProgressViewProps) => <ProgressView {...props} />;
