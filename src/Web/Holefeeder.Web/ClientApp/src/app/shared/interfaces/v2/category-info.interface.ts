import {ICategoryType} from "@app/shared/interfaces/v2/category-type.interface";

export interface ICategoryInfo {
  id: string;
  name: string;
  type: ICategoryType;
  color: string;
}
