import { type AsyncResult, tk } from '@holefeeder/shared/core';
import ChevronLeftIcon from '@mui/icons-material/ChevronLeft';
import { Alert, IconButton, ListItem, Skeleton, Stack, Typography } from '@mui/material';
import { Link } from 'react-router';
import { translate } from '@/shared/i18n/translate';
import { EmptyList } from '@/shared/presentation/EmptyList';
import { useTags } from '../api/use-tags';
import type { Tag } from '../domain/tag';
import { Rows, Section } from './SettingsList';

const counted = (count: number, one: string, other: string) => (count === 0 ? [] : [translate(count === 1 ? one : other, { count: count })]);

const usageOf = (tag: Tag) =>
  [
    ...counted(tag.transactionCount, tk.tags.transactionOne, tk.tags.transactionOther),
    ...counted(tag.activeCashflowCount, tk.tags.cashflowOne, tk.tags.cashflowOther),
  ].join(' · ');

const TagRow = ({ tag }: { tag: Tag }) => (
  <ListItem disablePadding data-testid="tags-row" sx={{ py: 1.25, minWidth: 0 }}>
    <Stack sx={{ minWidth: 0, flexGrow: 1 }}>
      <Typography data-testid="tags-row-name" noWrap sx={{ fontSize: 17 }}>
        {tag.tag}
      </Typography>
      <Typography data-testid="tags-row-usage" noWrap sx={{ fontSize: 13, color: 'text.secondary' }}>
        {usageOf(tag)}
      </Typography>
    </Stack>
  </ListItem>
);

const TagList = ({ tags }: { tags: AsyncResult<Tag[]> }) => {
  if (tags.isLoading) return <Skeleton variant="rounded" height={240} data-testid="tags-loading" sx={{ borderRadius: 3 }} />;

  if (tags.isFailure)
    return (
      <Alert severity="error" data-testid="tags-load-error">
        {translate(tk.tags.loadFailed)}
      </Alert>
    );

  if (tags.value.length === 0)
    return <EmptyList testID="tags-empty" title={translate(tk.tags.emptyTitle)} hint={translate(tk.tags.emptyHint)} />;

  return (
    <Section title={translate(tk.tagsSection.title)} testID="tags-list">
      <Rows>
        {tags.value.map((tag) => (
          <TagRow key={tag.tag} tag={tag} />
        ))}
      </Rows>
    </Section>
  );
};

export const TagsPage = () => {
  const tags = useTags();

  return (
    <Stack spacing={3.5} data-testid="tags-page" sx={{ maxWidth: 720 }}>
      <Stack direction="row" alignItems="center" spacing={1.5}>
        <IconButton
          component={Link}
          to="/settings"
          data-testid="tags-back"
          aria-label={translate(tk.tags.back)}
          sx={{ bgcolor: 'background.paper', color: 'text.primary', flexShrink: 0 }}
        >
          <ChevronLeftIcon />
        </IconButton>
        <Typography variant="h1" noWrap sx={{ flexGrow: 1, minWidth: 0, fontSize: 28, fontWeight: 700 }}>
          {translate(tk.tagsSection.title)}
        </Typography>
      </Stack>
      <TagList tags={tags} />
    </Stack>
  );
};
