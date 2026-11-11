import { tk } from '@holefeeder/shared/core';
import CloseIcon from '@mui/icons-material/Close';
import { Box, ButtonBase, InputBase, Stack, Typography } from '@mui/material';
import { useState } from 'react';
import { useTagSuggestions } from '@/shared/api/use-tag-suggestions';
import { translate } from '@/shared/i18n/translate';

const shownSuggestions = 8;

type TagsFieldProps = { testID: string; categoryId: string; tags: string[]; onChange: (tags: string[]) => void };

const chipSx = { px: 1.25, py: 0.5, borderRadius: 9999, fontSize: 13, fontWeight: 500, lineHeight: 1.2 };

export const TagsField = ({ testID, categoryId, tags, onChange }: TagsFieldProps) => {
  const [draft, setDraft] = useState('');
  const suggestions = useTagSuggestions(categoryId);

  const add = (tag: string) => {
    const value = tag.trim();
    if (value !== '' && !tags.includes(value)) onChange([...tags, value]);
  };

  const commitDraft = () => {
    add(draft);
    setDraft('');
  };

  const remove = (tag: string) => onChange(tags.filter((current) => current !== tag));

  const offered = suggestions.isSuccess ? suggestions.value.filter((tag) => !tags.includes(tag)).slice(0, shownSuggestions) : [];

  return (
    <Stack spacing={0.75}>
      <Typography component="label" htmlFor={`${testID}-tag-input`} sx={{ fontSize: 13, color: 'text.secondary' }}>
        {translate(tk.purchase.basicSection.tags)}
      </Typography>
      <Stack
        direction="row"
        useFlexGap
        alignItems="center"
        sx={{ flexWrap: 'wrap', gap: 0.75, px: 1.25, py: 1, border: 1, borderColor: 'divider', borderRadius: 2.5 }}
      >
        {tags.map((tag) => (
          <Stack
            key={tag}
            direction="row"
            alignItems="center"
            spacing={0.5}
            sx={{ ...chipSx, color: 'primary.main', bgcolor: (theme) => `rgba(${theme.vars!.palette.primary.mainChannel} / 0.15)` }}
          >
            <Box component="span" data-testid={`${testID}-tag`}>
              {tag}
            </Box>
            <ButtonBase
              aria-label={translate(tk.newTransaction.removeTag, { tag: tag })}
              onClick={() => remove(tag)}
              sx={{ borderRadius: 9999 }}
            >
              <CloseIcon sx={{ fontSize: 14 }} />
            </ButtonBase>
          </Stack>
        ))}
        <InputBase
          id={`${testID}-tag-input`}
          value={draft}
          placeholder={translate(tk.newTransaction.tagsPlaceholder)}
          onChange={(event) => setDraft(event.target.value)}
          onBlur={commitDraft}
          onKeyDown={(event) => {
            if (event.key === 'Enter' || event.key === ',') {
              event.preventDefault();
              commitDraft();
            }
          }}
          inputProps={{ 'data-testid': `${testID}-tag-input` }}
          sx={{ flex: 1, minWidth: 96, fontSize: 15 }}
        />
      </Stack>
      {offered.length > 0 && (
        <Stack direction="row" useFlexGap sx={{ flexWrap: 'wrap', gap: 0.75 }}>
          {offered.map((tag) => (
            <ButtonBase
              key={tag}
              aria-label={translate(tk.newTransaction.addTag, { tag: tag })}
              onClick={() => add(tag)}
              sx={{ ...chipSx, border: 1, borderColor: 'divider', color: 'text.secondary' }}
            >
              <Box component="span" aria-hidden>
                +&nbsp;
              </Box>
              <Box component="span" data-testid={`${testID}-suggestion`}>
                {tag}
              </Box>
            </ButtonBase>
          ))}
        </Stack>
      )}
    </Stack>
  );
};
