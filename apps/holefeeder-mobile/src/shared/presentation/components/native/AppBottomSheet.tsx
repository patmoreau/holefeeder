import { ExpoBottomSheet, ExpoBottomSheetProps } from '@/shared/presentation/components/native/expo/ExpoBottomSheet';

export type AppBottomSheetProps = ExpoBottomSheetProps & {};

export const AppBottomSheet = (props: AppBottomSheetProps) => {
  return <ExpoBottomSheet {...props} />;
};
