import { Snackbar, SnackbarContent } from '@mui/material';

export type Notify = (message: string) => void;

export const DashboardToast = ({ message, onClose }: { message: string | null; onClose: () => void }) => (
  <Snackbar
    open={message !== null}
    autoHideDuration={4000}
    onClose={(_, reason) => reason !== 'clickaway' && onClose()}
    anchorOrigin={{ vertical: 'bottom', horizontal: 'center' }}
  >
    <SnackbarContent data-testid="dashboard-toast" message={message} sx={{ borderRadius: 2.5, fontSize: 15 }} />
  </Snackbar>
);
