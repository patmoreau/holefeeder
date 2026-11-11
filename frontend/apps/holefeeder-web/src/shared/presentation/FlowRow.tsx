import { type DateOnly, LocalFormatter, today } from '@holefeeder/shared/core';
import LabelOutlinedIcon from '@mui/icons-material/LabelOutlined';
import { Box, ButtonBase, Stack, Typography } from '@mui/material';
import type { ReactNode } from 'react';
import { Language } from '@/shared/i18n/language';
import { translate } from '@/shared/i18n/translate';
import { currency } from './tone';

const lineHeight = 1.2;

export type Flow = {
  title: string;
  category: string | null;
  categoryColor: string;
  tags: string[];
  amount: number;
  date: DateOnly;
};

const TagChip = ({ tag, testID }: { tag: string; testID: string }) => (
  <Box
    component="span"
    data-testid={testID}
    sx={{
      px: 1,
      py: 0.25,
      borderRadius: 9999,
      fontSize: 13,
      lineHeight: lineHeight,
      color: 'primary.main',
      bgcolor: (theme) => `rgba(${theme.vars!.palette.primary.mainChannel} / 0.15)`,
    }}
  >
    {tag}
  </Box>
);

const FlowContent = ({ flow, testID }: { flow: Flow; testID: string }) => (
  <>
    <LabelOutlinedIcon data-testid={`${testID}-icon`} sx={{ fontSize: 20, color: flow.categoryColor }} />
    <Stack spacing={0.5} sx={{ flexGrow: 1, minWidth: 0 }}>
      <Typography data-testid={`${testID}-title`} noWrap sx={{ fontSize: 17, fontWeight: 600, lineHeight: lineHeight }}>
        {flow.title}
      </Typography>
      {(flow.category || flow.tags.length > 0) && (
        <Stack direction="row" alignItems="center" spacing={0.5} useFlexGap sx={{ flexWrap: 'wrap' }}>
          {flow.category && (
            <Typography data-testid={`${testID}-category`} sx={{ fontSize: 13, lineHeight: lineHeight, color: 'text.secondary' }}>
              {flow.category}
            </Typography>
          )}
          {flow.tags.map((tag) => (
            <TagChip key={tag} tag={tag} testID={`${testID}-tag`} />
          ))}
        </Stack>
      )}
    </Stack>
    <Stack spacing={0.25} alignItems="flex-end" sx={{ flexShrink: 0 }}>
      <Typography data-testid={`${testID}-amount`} sx={{ fontSize: 17, lineHeight: lineHeight }}>
        {currency(flow.amount)}
      </Typography>
      <Typography data-testid={`${testID}-date`} sx={{ fontSize: 13, lineHeight: lineHeight, color: 'text.secondary' }}>
        {LocalFormatter.date(flow.date, today(), Language.locale(), translate)}
      </Typography>
    </Stack>
  </>
);

type FlowRowProps = { flow: Flow; testID: string; action?: ReactNode; onOpen?: () => void };

export const FlowRow = ({ flow, testID, action, onOpen }: FlowRowProps) => (
  <Stack
    direction="row"
    alignItems="center"
    spacing={1.5}
    data-testid={`${testID}-row`}
    sx={{ py: onOpen ? 0.5 : 1.5, '& + &': { borderTop: 1, borderColor: 'divider' } }}
  >
    {onOpen ? (
      <ButtonBase
        data-testid={`${testID}-open`}
        aria-label={flow.title}
        onClick={onOpen}
        sx={{ flexGrow: 1, minWidth: 0, py: 1, gap: 1.5, justifyContent: 'flex-start', textAlign: 'left', borderRadius: 2 }}
      >
        <FlowContent flow={flow} testID={testID} />
      </ButtonBase>
    ) : (
      <FlowContent flow={flow} testID={testID} />
    )}
    {action}
  </Stack>
);
