import { CategoryType } from '../models';

export interface ICategoryInfo {
  id: string;
  name: string;
  type: CategoryType;
  color: string;
}
