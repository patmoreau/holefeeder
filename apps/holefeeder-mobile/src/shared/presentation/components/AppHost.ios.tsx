import { Host, HostProps, VStack } from '@expo/ui/swift-ui';
import { ignoreSafeArea } from '@expo/ui/swift-ui/modifiers';

export const AppHost = ({ matchContents = true, children, ...props }: HostProps) => {
  return (
    <Host matchContents={matchContents} {...props}>
      <VStack modifiers={[ignoreSafeArea()]}>{children}</VStack>
    </Host>
  );
};
