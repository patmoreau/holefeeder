type Titled = { description: string; categoryName: string };

const hasDescription = (description: string): boolean => description.trim().length > 0;

const title = ({ description, categoryName }: Titled): string => (hasDescription(description) ? description : categoryName);

const showsCategory = ({ description }: Pick<Titled, 'description'>): boolean => hasDescription(description);

export const FlowTitle = {
  title: title,
  showsCategory: showsCategory,
};
