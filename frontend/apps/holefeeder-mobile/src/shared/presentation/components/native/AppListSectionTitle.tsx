import { AppModifiers } from '@/shared/presentation/components/native/AppModifiers';
import { AppText } from '@/shared/presentation/components/native/AppText';
import { spacing } from '@/types/theme/design-tokens';

export type AppListSectionTitleProps = {
  title: string;
};

// A heading that scrolls with the list, unlike a SwiftUI `Section` header: those stay pinned to
// the top and paint a blur material behind themselves, which costs a frame on every scroll tick
// and renders differently on device than in the simulator. Use this where a section's heading is
// decoration rather than a landmark to scroll back to.
export const AppListSectionTitle = ({ title }: AppListSectionTitleProps) => (
  <AppText
    variant={'subtitle'}
    modifiers={[
      AppModifiers.hideListRowSeparator,
      AppModifiers.listRowBackground('clear'),
      AppModifiers.listRowInsets({ top: spacing.lg, leading: spacing.lg, bottom: spacing.xs, trailing: spacing.lg }),
    ]}
  >
    {title}
  </AppText>
);
