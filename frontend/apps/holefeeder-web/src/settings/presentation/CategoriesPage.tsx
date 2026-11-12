import { type AsyncResult, tk } from '@holefeeder/shared/core';
import AddIcon from '@mui/icons-material/Add';
import ChevronLeftIcon from '@mui/icons-material/ChevronLeft';
import StarRoundedIcon from '@mui/icons-material/StarRounded';
import { Alert, Box, Button, ButtonBase, IconButton, ListItem, Skeleton, Stack, Typography } from '@mui/material';
import { useState } from 'react';
import { Link } from 'react-router';
import { useApiRefresh } from '@/shared/api/api-refresh';
import { ApiRefreshProvider } from '@/shared/api/ApiRefreshProvider';
import { translate } from '@/shared/i18n/translate';
import { DashboardToast, type Notify } from '@/shared/presentation/DashboardToast';
import { DeleteDialog } from '@/shared/presentation/DeleteDialog';
import { EmptyList } from '@/shared/presentation/EmptyList';
import { RowActionsMenu } from '@/shared/presentation/RowActionsMenu';
import { Rows, Section } from '@/shared/presentation/Section';
import { currency, roundsToZero } from '@/shared/presentation/tone';
import { useCategories } from '../api/use-categories';
import { useDeleteCategory } from '../api/use-delete-category';
import type { Category } from '../domain/category';
import { CategoryFlyout, type CategoryEditing } from './CategoryFlyout';

const secondaryText = { fontSize: 15, color: 'text.secondary', flexShrink: 0 };

const CategoryDetails = ({ category }: { category: Category }) => (
  <>
    <Box aria-hidden sx={{ width: 12, height: 12, borderRadius: '50%', bgcolor: category.color, flexShrink: 0 }} />
    <Typography noWrap sx={{ fontSize: 17, minWidth: 0 }}>
      {category.name}
    </Typography>
    {category.favorite && (
      <StarRoundedIcon aria-label={translate(tk.categories.favorite)} sx={{ fontSize: 18, color: 'warning.main', flexShrink: 0 }} />
    )}
    <Box sx={{ flexGrow: 1 }} />
    {category.system && <Typography sx={secondaryText}>{translate(tk.categories.system)}</Typography>}
    {!category.system && !roundsToZero(category.budgetAmount) && (
      <Typography data-testid="categories-row-budget" sx={secondaryText}>
        {currency(category.budgetAmount)}
      </Typography>
    )}
  </>
);

const rowContent = { display: 'flex', alignItems: 'center', gap: 1.5, py: 1.5, minWidth: 0, flexGrow: 1 };

type CategoryActions = { onOpen: (category: Category) => void; onDelete: (category: Category) => void };

type CategoryRowProps = { category: Category } & CategoryActions;

const CategoryRow = ({ category, onOpen, onDelete }: CategoryRowProps) => (
  <ListItem disablePadding data-testid="categories-row" sx={{ gap: 1, minWidth: 0 }}>
    {category.system ? (
      <Box sx={rowContent}>
        <CategoryDetails category={category} />
      </Box>
    ) : (
      <ButtonBase
        data-testid="categories-row-open"
        onClick={() => onOpen(category)}
        sx={{ ...rowContent, justifyContent: 'flex-start', textAlign: 'left' }}
      >
        <CategoryDetails category={category} />
      </ButtonBase>
    )}
    {!category.system && <RowActionsMenu testID="categories-row" onEdit={() => onOpen(category)} onDelete={() => onDelete(category)} />}
  </ListItem>
);

type CategorySectionProps = { title: string; testID: string; categories: Category[] } & CategoryActions;

const CategorySection = ({ title, testID, categories, ...actions }: CategorySectionProps) =>
  categories.length === 0 ? null : (
    <Section title={title} testID={testID}>
      <Rows>
        {categories.map((category) => (
          <CategoryRow key={category.id} category={category} {...actions} />
        ))}
      </Rows>
    </Section>
  );

const CategoryList = ({ categories, ...actions }: { categories: AsyncResult<Category[]> } & CategoryActions) => {
  if (categories.isLoading) return <Skeleton variant="rounded" height={240} data-testid="categories-loading" sx={{ borderRadius: 3 }} />;

  if (categories.isFailure)
    return (
      <Alert severity="error" data-testid="categories-load-error">
        {translate(tk.categories.loadFailed)}
      </Alert>
    );

  const userCategories = categories.value.filter((category) => !category.system);
  const ofType = (type: Category['type']) => userCategories.filter((category) => category.type === type);

  return (
    <Stack spacing={3.5}>
      {userCategories.length === 0 && (
        <EmptyList testID="categories-empty" title={translate(tk.categories.emptyTitle)} hint={translate(tk.categories.emptyHint)} />
      )}
      <CategorySection title={translate(tk.categories.expenses)} testID="categories-expenses" categories={ofType('expense')} {...actions} />
      <CategorySection title={translate(tk.categories.income)} testID="categories-income" categories={ofType('gain')} {...actions} />
      <CategorySection
        title={translate(tk.categories.system)}
        testID="categories-system"
        categories={categories.value.filter((category) => category.system)}
        {...actions}
      />
    </Stack>
  );
};

const actionButtonSx = {
  flexShrink: 0,
  minWidth: 0,
  px: { xs: 1.25, md: 2 },
  py: 1,
  borderRadius: 2.5,
  fontSize: 15,
  fontWeight: 600,
  textTransform: 'none',
};

const CategoriesContent = ({ notify }: { notify: Notify }) => {
  const categories = useCategories();
  const [editing, setEditing] = useState<CategoryEditing>(null);
  const [deleting, setDeleting] = useState<Category | null>(null);
  const deleteCategory = useDeleteCategory();
  const { refresh } = useApiRefresh();

  const confirmDelete = async (category: Category) => {
    setDeleting(null);
    const result = await deleteCategory(category.id);
    notify(translate(result.isSuccess ? tk.categories.deleted : tk.categories.deleteFailed, { name: category.name }));
    if (result.isSuccess) refresh();
  };

  return (
    <Stack spacing={3.5} data-testid="categories-page" sx={{ maxWidth: 720 }}>
      <Stack direction="row" alignItems="center" spacing={1.5}>
        <IconButton
          component={Link}
          to="/settings"
          data-testid="categories-back"
          aria-label={translate(tk.categories.back)}
          sx={{ bgcolor: 'background.paper', color: 'text.primary', flexShrink: 0 }}
        >
          <ChevronLeftIcon />
        </IconButton>
        <Typography variant="h1" noWrap sx={{ flexGrow: 1, minWidth: 0, fontSize: 28, fontWeight: 700 }}>
          {translate(tk.categoriesSection.title)}
        </Typography>
        <Button
          variant="contained"
          disableElevation
          data-testid="categories-new"
          aria-label={translate(tk.categories.newCategory)}
          onClick={() => setEditing({ category: null })}
          sx={actionButtonSx}
        >
          <AddIcon fontSize="small" sx={{ mr: { md: 1 } }} />
          <Box component="span" sx={{ display: { xs: 'none', md: 'inline' } }}>
            {translate(tk.categories.newCategory)}
          </Box>
        </Button>
      </Stack>
      <CategoryList categories={categories} onOpen={(category) => setEditing({ category: category })} onDelete={setDeleting} />
      <CategoryFlyout editing={editing} onClose={() => setEditing(null)} notify={notify} />
      {deleting !== null && (
        <DeleteDialog
          open
          testID="category-delete"
          title={translate(tk.categories.deleteTitle, { name: deleting.name })}
          message={translate(tk.categories.deleteMessage, { name: deleting.name })}
          onCancel={() => setDeleting(null)}
          onConfirm={() => void confirmDelete(deleting)}
        />
      )}
    </Stack>
  );
};

export const CategoriesPage = () => {
  const [toast, setToast] = useState<string | null>(null);

  return (
    <ApiRefreshProvider>
      <CategoriesContent notify={setToast} />
      <DashboardToast message={toast} onClose={() => setToast(null)} />
    </ApiRefreshProvider>
  );
};
