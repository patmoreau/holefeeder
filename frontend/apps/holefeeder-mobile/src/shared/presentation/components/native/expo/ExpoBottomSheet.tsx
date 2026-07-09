import { BottomSheet, BottomSheetProps } from '@expo/ui';

export type ExpoBottomSheetProps = BottomSheetProps & {};

export const ExpoBottomSheet = (props: ExpoBottomSheetProps) => {
  return <BottomSheet {...props} />;
};
