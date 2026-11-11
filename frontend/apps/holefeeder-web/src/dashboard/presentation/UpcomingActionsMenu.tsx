import { tk } from '@holefeeder/shared/core';
import MoreVertIcon from '@mui/icons-material/MoreVert';
import { Divider, IconButton, Menu, MenuItem } from '@mui/material';
import { useState } from 'react';
import { translate } from '@/shared/i18n/translate';

type UpcomingActionsMenuProps = {
  disabled: boolean;
  onPayWithChanges?: () => void;
  onClear: () => void;
  onDelete: () => void;
};

export const UpcomingActionsMenu = ({ disabled, onPayWithChanges, onClear, onDelete }: UpcomingActionsMenuProps) => {
  const [anchor, setAnchor] = useState<HTMLElement | null>(null);
  const close = () => setAnchor(null);
  const choose = (action?: () => void) => () => {
    close();
    action?.();
  };

  return (
    <>
      <IconButton
        size="small"
        disabled={disabled}
        aria-label={translate(tk.payUpcoming.moreActions)}
        aria-haspopup="menu"
        aria-expanded={anchor !== null}
        data-testid="dashboard-upcoming-more"
        onClick={(event) => setAnchor(event.currentTarget)}
        sx={{ flexShrink: 0, borderRadius: 2, color: 'text.secondary', bgcolor: anchor ? 'action.selected' : undefined }}
      >
        <MoreVertIcon fontSize="small" />
      </IconButton>
      <Menu
        anchorEl={anchor}
        open={anchor !== null}
        onClose={close}
        anchorOrigin={{ vertical: 'bottom', horizontal: 'right' }}
        transformOrigin={{ vertical: 'top', horizontal: 'right' }}
        slotProps={{ paper: { sx: { minWidth: 232, borderRadius: 2.5 } } }}
      >
        <MenuItem data-testid="dashboard-upcoming-pay-with-changes" disabled={!onPayWithChanges} onClick={choose(onPayWithChanges)}>
          {translate(tk.payUpcoming.payWithChanges)}
        </MenuItem>
        <MenuItem data-testid="dashboard-upcoming-clear" onClick={choose(onClear)}>
          {translate(tk.swipeableActions.clear)}
        </MenuItem>
        <Divider />
        <MenuItem data-testid="dashboard-upcoming-delete" onClick={choose(onDelete)} sx={{ color: 'error.main' }}>
          {translate(tk.swipeableActions.delete)}
        </MenuItem>
      </Menu>
    </>
  );
};
