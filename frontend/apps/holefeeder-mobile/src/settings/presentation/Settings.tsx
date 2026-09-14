import { View } from 'react-native';
import { SettingsContent } from '@/settings/presentation/SettingsContent';

// No top padding, unlike AppScreen: the form scrolls underneath the transparent header rather
// than starting below it, so the header shows the content passing behind it instead of a band
// of the screen's background colour.
const SettingsScreen = () => {
  return (
    <View style={{ flex: 1 }}>
      <SettingsContent />
    </View>
  );
};

export default SettingsScreen;
