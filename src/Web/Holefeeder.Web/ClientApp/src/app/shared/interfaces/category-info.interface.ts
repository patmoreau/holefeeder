import {CategoryType} from "@app/shared";

export interface ICategoryInfo {
  id: string;
  name: string;
  type: CategoryType;
  color: string;
}
