import { type AsyncResult, tk } from '@holefeeder/shared/core';
import ChevronLeftIcon from '@mui/icons-material/ChevronLeft';
import { Alert, IconButton, ListItem, Skeleton, Stack, Typography } from '@mui/material';
import { useState } from 'react';
import { Link } from 'react-router';
import { useApiRefresh } from '@/shared/api/api-refresh';
import { ApiRefreshProvider } from '@/shared/api/ApiRefreshProvider';
import { translate } from '@/shared/i18n/translate';
import { DashboardToast, type Notify } from '@/shared/presentation/DashboardToast';
import { DeleteDialog } from '@/shared/presentation/DeleteDialog';
import { EmptyList } from '@/shared/presentation/EmptyList';
import { RowActionsMenu } from '@/shared/presentation/RowActionsMenu';
import { useDeleteTag } from '../api/use-delete-tag';
import { useTags } from '../api/use-tags';
import type { Tag } from '../domain/tag';
import { Rows, Section } from './SettingsList';
import { TagRenameDialog } from './TagRenameDialog';

const counted = (count: number, one: string, other: string) => (count === 0 ? [] : [translate(count === 1 ? one : other, { count: count })]);

const usagesOf = (tag: Tag) => [
  ...counted(tag.transactionCount, tk.tags.transactionOne, tk.tags.transactionOther),
  ...counted(tag.activeCashflowCount, tk.tags.cashflowOne, tk.tags.cashflowOther),
];

const usageOf = (tag: Tag) => usagesOf(tag).join(' · ');

const usageSentenceOf = (tag: Tag) => {
  const [first, second] = usagesOf(tag);
  return second === undefined ? first : translate(tk.tags.usageAnd, { first: first, second: second });
};

type TagActions = { onRename: (tag: Tag) => void; onDelete: (tag: Tag) => void };

const TagRow = ({ tag, onRename, onDelete }: { tag: Tag } & TagActions) => (
  <ListItem disablePadding data-testid="tags-row" sx={{ py: 1.25, gap: 1, minWidth: 0 }}>
    <Stack sx={{ minWidth: 0, flexGrow: 1 }}>
      <Typography data-testid="tags-row-name" noWrap sx={{ fontSize: 17 }}>
        {tag.tag}
      </Typography>
      <Typography data-testid="tags-row-usage" noWrap sx={{ fontSize: 13, color: 'text.secondary' }}>
        {usageOf(tag)}
      </Typography>
    </Stack>
    <RowActionsMenu testID="tags-row" onEdit={() => onRename(tag)} onDelete={() => onDelete(tag)} editLabel={translate(tk.tags.renameAction)} />
  </ListItem>
);

const TagList = ({ tags, ...actions }: { tags: AsyncResult<Tag[]> } & TagActions) => {
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
          <TagRow key={tag.tag} tag={tag} {...actions} />
        ))}
      </Rows>
    </Section>
  );
};

const TagsContent = ({ notify }: { notify: Notify }) => {
  const tags = useTags();
  const [renaming, setRenaming] = useState<Tag | null>(null);
  const [deleting, setDeleting] = useState<Tag | null>(null);
  const deleteTag = useDeleteTag();
  const { refresh } = useApiRefresh();

  const renamed = (tag: Tag) => {
    setRenaming(null);
    notify(translate(tk.tags.renamed, { name: tag.tag }));
    refresh();
  };

  const confirmDelete = async (tag: Tag) => {
    setDeleting(null);
    const result = await deleteTag(tag.tag);
    notify(translate(result.isSuccess ? tk.tags.deleted : tk.tags.deleteFailed, { name: tag.tag }));
    if (result.isSuccess) refresh();
  };

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
      <TagList tags={tags} onRename={setRenaming} onDelete={setDeleting} />
      {renaming !== null && <TagRenameDialog tag={renaming} onClose={() => setRenaming(null)} onRenamed={renamed} />}
      {deleting !== null && (
        <DeleteDialog
          open
          testID="tag-delete-dialog"
          title={translate(tk.tags.deleteTitle, { name: deleting.tag })}
          message={translate(tk.tags.deleteMessage, { name: deleting.tag, usage: usageSentenceOf(deleting) })}
          onCancel={() => setDeleting(null)}
          onConfirm={() => void confirmDelete(deleting)}
        />
      )}
    </Stack>
  );
};

export const TagsPage = () => {
  const [toast, setToast] = useState<string | null>(null);

  return (
    <ApiRefreshProvider>
      <TagsContent notify={setToast} />
      <DashboardToast message={toast} onClose={() => setToast(null)} />
    </ApiRefreshProvider>
  );
};
