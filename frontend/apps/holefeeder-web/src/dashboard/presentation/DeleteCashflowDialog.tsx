import { tk } from '@holefeeder/shared/core';
import { Button, Dialog, DialogActions, DialogContent, DialogContentText, DialogTitle } from '@mui/material';
import { translate } from '@/shared/i18n/translate';

type DeleteCashflowDialogProps = {
  title: string | null;
  onCancel: () => void;
  onConfirm: () => void;
};

const actionButton = {
  flex: { xs: 1, sm: 'none' },
  px: 2.5,
  py: 1.25,
  borderRadius: 2.5,
  fontSize: 15,
  fontWeight: 600,
  textTransform: 'none',
};

export const DeleteCashflowDialog = ({ title, onCancel, onConfirm }: DeleteCashflowDialogProps) => (
  <Dialog
    open={title !== null}
    onClose={onCancel}
    slotProps={{ paper: { sx: { borderRadius: 4, width: 400, maxWidth: 'calc(100% - 48px)', m: 3 } } }}
  >
    <DialogTitle sx={{ fontSize: 20, fontWeight: 700, pt: 3, px: 3, pb: 1.5 }}>
      {translate(tk.payUpcoming.deleteTitle, { title: title ?? '' })}
    </DialogTitle>
    <DialogContent sx={{ px: 3 }}>
      <DialogContentText sx={{ fontSize: 15 }}>{translate(tk.payUpcoming.deleteMessage, { title: title ?? '' })}</DialogContentText>
    </DialogContent>
    <DialogActions sx={{ px: 3, pb: 2.5, pt: 1, gap: 1.5, '& > :not(style) ~ :not(style)': { ml: 0 } }}>
      <Button
        variant="outlined"
        color="inherit"
        data-testid="dashboard-delete-cancel"
        onClick={onCancel}
        sx={{ ...actionButton, borderColor: 'divider' }}
      >
        {translate(tk.common.cancel)}
      </Button>
      <Button variant="contained" color="error" disableElevation data-testid="dashboard-delete-confirm" onClick={onConfirm} sx={actionButton}>
        {translate(tk.swipeableActions.delete)}
      </Button>
    </DialogActions>
  </Dialog>
);
