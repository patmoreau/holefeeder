import { AppButtonVariant } from '@/shared/presentation/components/AppButtonVariant';
import { AppButton } from '@/shared/presentation/components/native/AppButton';
import { AppColumn } from '@/shared/presentation/components/native/AppColumn';
import { AppIcon } from '@/shared/presentation/components/native/AppIcon';
import { AppModifiers } from '@/shared/presentation/components/native/AppModifiers';
import { AppText } from '@/shared/presentation/components/native/AppText';
import { UniversalIcon } from '@/shared/presentation/core/app-icon-map';
import { useTheme } from '@/shared/theme/core/use-theme';

export type EmptyStateProps = {
  icon: UniversalIcon;
  title: string;
  hint: string;
  action?: { label: string; onPress: () => void; testID?: string };
  testID?: string;
};

export const EmptyState = ({ icon, title, hint, action, testID }: EmptyStateProps) => {
  const { theme } = useTheme();

  return (
    <AppColumn
      spacing={8}
      alignment={'center'}
      testID={testID}
      modifiers={[AppModifiers.fillWidth, AppModifiers.hideListRowSeparator, AppModifiers.listRowBackground('clear')]}
    >
      <AppColumn
        alignment={'center'}
        modifiers={[
          AppModifiers.frame({ width: 56, height: 56 }),
          AppModifiers.background(`${theme.colors.primary}1A`),
          AppModifiers.cornerRadius(28),
        ]}
      >
        <AppIcon name={icon.ios} size={26} color={theme.colors.primary} />
      </AppColumn>
      <AppText variant={'defaultSemiBold'} textStyle={{ textAlign: 'center' }}>
        {title}
      </AppText>
      <AppText variant={'footnote'} textStyle={{ textAlign: 'center' }}>
        {hint}
      </AppText>
      {action && <AppButton variant={AppButtonVariant.primary} label={action.label} onPress={action.onPress} testID={action.testID} />}
    </AppColumn>
  );
};
