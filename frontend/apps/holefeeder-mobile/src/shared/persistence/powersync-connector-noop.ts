import { PowerSyncBackendConnector } from '@powersync/common';

export const PowersyncConnectorNoop = (): PowerSyncBackendConnector => {
  const fetchCredentials = async () => {
    return {
      endpoint: '',
      token: '',
    };
  };

  const uploadData = async () => {};

  return {
    fetchCredentials,
    uploadData,
  };
};
