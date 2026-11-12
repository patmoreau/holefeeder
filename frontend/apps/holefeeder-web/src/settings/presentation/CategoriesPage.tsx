import { type AsyncResult, tk } from '@holefeeder/shared/core';
import ChevronLeftIcon from '@mui/icons-material/ChevronLeft';
import StarRoundedIcon from '@mui/icons-material/StarRounded';
import { Alert, Box, IconButton, ListItem, Skeleton, Stack, Typography } from '@mui/material';
import { useState } from 'react';
import { Link } from 'react-router';
import { ApiRefreshProvider } from '@/shared/api/ApiRefreshProvider';
import { translate } from '@/shared/i18n/translate';
import { DashboardToast } from '@/shared/presentation/DashboardToast';
import { EmptyList } from '@/shared/presentation/EmptyList';
import { currency, roundsToZero } from '@/shared/presentation/tone';
import { useCategories } from '../api/use-categories';
import type { Category } from '../domain/category';
import { Rows, Section } from './SettingsList';

const secondaryText = { fontSize: 15, color: 'text.secondary', flexShrink: 0 };

const CategoryRow = ({ category }: { category: Category }) => (
  <ListItem disablePadding data-testid="categories-row" sx={{ py: 1.5, gap: 1.5, minWidth: 0 }}>
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
  </ListItem>
);

const CategorySection = ({ title, testID, categories }: { title: string; testID: string; categories: Category[] }) =>
  categories.length === 0 ? null : (
    <Section title={title} testID={testID}>
      <Rows>
        {categories.map((category) => (
          <CategoryRow key={category.id} category={category} />
        ))}
      </Rows>
    </Section>
  );

const CategoryList = ({ categories }: { categories: AsyncResult<Category[]> }) => {
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
      <CategorySection title={translate(tk.categories.expenses)} testID="categories-expenses" categories={ofType('expense')} />
      <CategorySection title={translate(tk.categories.income)} testID="categories-income" categories={ofType('gain')} />
      <CategorySection
        title={translate(tk.categories.system)}
        testID="categories-system"
        categories={categories.value.filter((category) => category.system)}
      />
    </Stack>
  );
};

const CategoriesContent = () => {
  const categories = useCategories();

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
      </Stack>
      <CategoryList categories={categories} />
    </Stack>
  );
};

export const CategoriesPage = () => {
  const [toast, setToast] = useState<string | null>(null);

  return (
    <ApiRefreshProvider>
      <CategoriesContent />
      <DashboardToast message={toast} onClose={() => setToast(null)} />
    </ApiRefreshProvider>
  );
};
