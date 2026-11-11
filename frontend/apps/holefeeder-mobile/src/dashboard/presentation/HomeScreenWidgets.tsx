import { useEffect, useState } from 'react';
import { BillsDueWidgetPublisher } from '@/dashboard/presentation/bills-due-widget/BillsDueWidgetPublisher';
import { installWidgetLogo } from '@/dashboard/widgets/install-widget-logo';

export const HomeScreenWidgets = () => {
  const [logo, setLogo] = useState<string>();

  useEffect(() => {
    installWidgetLogo().then((result) => {
      if (result.isSuccess) setLogo(result.value);
    });
  }, []);

  return <BillsDueWidgetPublisher logo={logo} />;
};
