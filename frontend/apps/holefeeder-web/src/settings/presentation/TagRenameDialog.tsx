import { tk } from '@holefeeder/shared/core';
import { Alert, Button, Dialog, DialogActions, DialogContent, DialogContentText, DialogTitle, Stack, TextField } from '@mui/material';
import { useState } from 'react';
import { translate } from '@/shared/i18n/translate';
import { useRenameTag } from '../api/use-rename-tag';
import type { Tag } from '../domain/tag';

const testID = 'tag-rename-dialog';

const actionButton = {
  flex: { xs: 1, sm: 'none' },
  px: 2.5,
  py: 1.25,
  borderRadius: 2.5,
  fontSize: 15,
  fontWeight: 600,
  textTransform: 'none',
};

type TagRenameDialogProps = { tag: Tag; onClose: () => void; onRenamed: (tag: Tag) => void };

export const TagRenameDialog = ({ tag, onClose, onRenamed }: TagRenameDialogProps) => {
  const [name, setName] = useState(tag.tag);
  const [failed, setFailed] = useState(false);
  const [saving, setSaving] = useState(false);
  const renameTag = useRenameTag();
  const newTag = name.trim();
  const canRename = newTag !== '' && !newTag.includes(',') && !saving;

  const rename = async () => {
    setSaving(true);
    const result = await renameTag(tag.tag, newTag);
    setSaving(false);
    if (result.isSuccess) onRenamed(tag);
    else setFailed(true);
  };

  return (
    <Dialog
      open
      onClose={onClose}
      data-testid={testID}
      slotProps={{ paper: { sx: { borderRadius: 4, width: 400, maxWidth: 'calc(100% - 48px)', m: 3 } } }}
    >
      <DialogTitle sx={{ fontSize: 20, fontWeight: 700, pt: 3, px: 3, pb: 1.5 }}>
        {translate(tk.tags.renameTitle, { name: tag.tag })}
      </DialogTitle>
      <DialogContent sx={{ px: 3 }}>
        <Stack spacing={2} sx={{ pt: 1 }}>
          <TextField
            autoFocus
            label={translate(tk.tags.name)}
            value={name}
            onChange={(event) => setName(event.target.value)}
            slotProps={{ inputLabel: { shrink: true }, htmlInput: { 'data-testid': `${testID}-name` } }}
          />
          <DialogContentText sx={{ fontSize: 13 }}>{translate(tk.tags.renameHint)}</DialogContentText>
          {failed && (
            <Alert severity="error" data-testid={`${testID}-error`}>
              {translate(tk.tags.renameFailed)}
            </Alert>
          )}
        </Stack>
      </DialogContent>
      <DialogActions sx={{ px: 3, pb: 2.5, pt: 1, gap: 1.5, '& > :not(style) ~ :not(style)': { ml: 0 } }}>
        <Button
          variant="outlined"
          color="inherit"
          data-testid={`${testID}-cancel`}
          onClick={onClose}
          sx={{ ...actionButton, borderColor: 'divider' }}
        >
          {translate(tk.common.cancel)}
        </Button>
        <Button
          variant="contained"
          disableElevation
          disabled={!canRename}
          data-testid={`${testID}-confirm`}
          onClick={() => void rename()}
          sx={actionButton}
        >
          {translate(tk.tags.rename)}
        </Button>
      </DialogActions>
    </Dialog>
  );
};
