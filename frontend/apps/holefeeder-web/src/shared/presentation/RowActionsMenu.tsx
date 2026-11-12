import { tk } from '@holefeeder/shared/core';
import MoreVertIcon from '@mui/icons-material/MoreVert';
import { Divider, IconButton, Menu, MenuItem } from '@mui/material';
import { useState } from 'react';
import { translate } from '@/shared/i18n/translate';

type RowActionsMenuProps = { testID: string; onEdit: () => void; onDelete: () => void; deleteLabel?: string };

export const RowActionsMenu = ({ testID, onEdit, onDelete, deleteLabel = translate(tk.swipeableActions.delete) }: RowActionsMenuProps) => {
  const [anchor, setAnchor] = useState<HTMLElement | null>(null);
  const close = () => setAnchor(null);
  const choose = (action: () => void) => () => {
    close();
    action();
  };

  return (
    <>
      <IconButton
        size="small"
        aria-label={translate(tk.payUpcoming.moreActions)}
        aria-haspopup="menu"
        aria-expanded={anchor !== null}
        data-testid={`${testID}-more`}
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
        <MenuItem data-testid={`${testID}-edit`} onClick={choose(onEdit)}>
          {translate(tk.editTransaction.edit)}
        </MenuItem>
        <Divider />
        <MenuItem data-testid={`${testID}-delete`} onClick={choose(onDelete)} sx={{ color: 'error.main' }}>
          {deleteLabel}
        </MenuItem>
      </Menu>
    </>
  );
};
