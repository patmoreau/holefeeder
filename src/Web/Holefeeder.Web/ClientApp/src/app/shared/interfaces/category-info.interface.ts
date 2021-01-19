import {CategoryType} from '../enums/category-type.enum';

export interface ICategoryInfo {
  id: string;
  name: string;
  type: CategoryType;
  color: string;
}
