import { LocalFormatter } from '@holefeeder/shared/core';
import { StyleSheet, View } from 'react-native';
import { AppIcon } from '@/shared/presentation/components/native/AppIcon';
import { AppNative } from '@/shared/presentation/components/native/AppNative';
import { AppText } from '@/shared/presentation/components/native/AppText';
import { AppIconMap } from '@/shared/presentation/core/app-icon-map';
import { useLocaleFormatter } from '@/shared/presentation/core/use-local-formatter';
import { useStyles } from '@/shared/theme/core/use-styles';
import { TagSpending } from '@/statistics/core/tag-spending';
import { Theme } from '@/types/theme/theme';

type TagSpendingCardProps = {
  item: TagSpending;
};

const createStyles = (theme: Theme) =>
  StyleSheet.create({
    container: {
      paddingVertical: 8,
      paddingHorizontal: 16,
    },
    row: {
      flexDirection: 'row',
      alignItems: 'center',
      justifyContent: 'space-between',
    },
    nameRow: {
      flexDirection: 'row',
      alignItems: 'center',
      gap: 8,
      flex: 1,
    },
    tagName: {
      color: theme.colors.text,
      flex: 1,
    },
    amount: {
      color: theme.colors.text,
    },
  });

export const TagSpendingCard = ({ item }: TagSpendingCardProps) => {
  const styles = useStyles(createStyles);
  const { currentLocale, currencyCode } = useLocaleFormatter();

  return (
    <View style={styles.container}>
      <View style={styles.row}>
        <View style={styles.nameRow}>
          <AppNative>
            <AppIcon size={16} name={AppIconMap.tag} />
          </AppNative>
          <AppText variant="default" style={styles.tagName}>
            {item.tag}
          </AppText>
        </View>
        <AppText variant="defaultSemiBold" style={styles.amount}>
          {LocalFormatter.currency(item.spentAmount, currentLocale, currencyCode)}
        </AppText>
      </View>
    </View>
  );
};
